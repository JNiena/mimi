using Azure.AI.OpenAI;
using Microsoft.CognitiveServices.Speech;

class Mimi
{
	public OpenAIClient MimiAI { get; }
	public MimiAIOptions AIOptions { get; set; }
	public MimiSynthesizerOptions SynthesizerOptions { get; set; }

	public Mimi(MimiAIOptions AIOptions, MimiSynthesizerOptions synthesizerOptions)
	{
		this.AIOptions = AIOptions;
		this.SynthesizerOptions = synthesizerOptions;
		MimiAI = new OpenAIClient(this.AIOptions.Key);
	}

	public async Task<string> CreateCompletion(ChatMessage[] messages) {
		var response = await MimiAI.GetChatCompletionsAsync(AIOptions.Model, new ChatCompletionsOptions {
			MaxTokens = AIOptions.MaxTokens,
			Temperature = AIOptions.Temperature,
			NucleusSamplingFactor = AIOptions.NucleusSamplingFactor,
            Messages = messages
		});
		return response.Value.Choices[0].Text;
	}

	public async Task SynthesizeAudioAsync(string text, string file)
	{
		var config = SpeechConfig.FromSubscription(SynthesizerOptions.Key, SynthesizerOptions.Region);

		var ssml = @$"
		<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
			<voice name='{SynthesizerOptions.Voice}'>
				<prosody pitch='{SynthesizerOptions.Pitch ?? "+0%"}' rate='{SynthesizerOptions.Rate ?? "+0%"}'>
					{text}
				</prosody>
			</voice>
		</speak>";

        using var synthesizer = new SpeechSynthesizer(config, null);
        var result = await synthesizer.SpeakSsmlAsync(ssml);

		using var stream = AudioDataStream.FromResult(result);
		await stream.SaveToWaveFileAsync(file);
    }
}

class MimiSynthesizerOptions
{
	public required string Key { get; init; }
	public required string Region { get; set; }
	public required string Voice { get; set; }
	public string? Pitch { get; set; }
	public string? Rate { get; set; }
}

class MimiAIOptions
{
	public required string Key { get; init; }
	public required string Model { get; set; }
	public int? MaxTokens { get; set; }
	public float? Temperature { get; set; }
	public float? NucleusSamplingFactor { get; set; } 
}