using System.Diagnostics;

class Program
{
	public static async Task Main()
	{
		var OPENAI_KEY = GetKey("OPENAI_KEY");
		var SPEECH_KEY = GetKey("SPEECH_KEY");

		var AIOptions = new MimiAIOptions {
			Key = OPENAI_KEY,
			Model = "gpt-3.5-turbo",
			MaxTokens = 128,
			NucleusSamplingFactor = 0.9f
		};

		var synthesizerOptions = new MimiSynthesizerOptions {
			Key = SPEECH_KEY,
			Region = "eastus",
			Voice = "en-US-AshleyNeural",
			Pitch = "+20%",
			Rate = "+10%"
		};

		var mimi = new Mimi(AIOptions, synthesizerOptions);
		var response = await mimi.CreateCompletion("How are you doing?");
		await mimi.SynthesizeAudioAsync(response, "synthesized.wav");
		Process.Start("ffplay", "-i synthesized.wav -autoexit -nodisp -hide_banner -loglevel error");
		
	}

	private static string GetKey(string name, string? defaultKey)
	{
		var key = Environment.GetEnvironmentVariable(name);
		if (key != null)
		{
			return key;
		}
		else if (defaultKey != null)
		{
			return defaultKey;
		}
		else
		{
			throw new Exception($"Environment variable {name} not found.");
		}
    }
}