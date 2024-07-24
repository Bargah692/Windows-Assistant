using System;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Speech.Synthesis;
using WindowsInput;
using WindowsInput.Native;

var recognizer = new SpeechRecognitionEngine();

// Grammar list
Choices commands = new Choices();
commands.Add(new string[]
{
    "apprentice open youtube",
    "apprentice open hollow bastion",
    "apprentice press enter",
    "apprentice open my email",
    "apprentice pause listening",
    "apprentice resume listening",
    "apprentice toggle media",
    "apprentice toggle track",
    "apprentice volume down",
    "apprentice volume up",
    "apprentice skip track",
    "apprentice previous track",
    "apprentice delete all text",
    "apprentice backspace",
    "apprentice command list",
    "apprentice toggle volume"
});

GrammarBuilder gb = new GrammarBuilder();
gb.Append(commands);
Grammar g = new Grammar(gb);
recognizer.LoadGrammar(g);
SpeechSynthesizer synthesizer = new SpeechSynthesizer();
synthesizer.Volume = 100;  // 0...100
synthesizer.Rate = 0;      // -10...10
var sim = new InputSimulator();
bool listening = true;

recognizer.SpeechRecognized += (sender, e) =>
{
    float confidence = e.Result.Confidence;

    if (confidence >= 0.94)
    {
        if (!listening && e.Result.Text == "apprentice resume listening")
        {
            listening = true;
            synthesizer.Speak("Listening Resumed");
            return;
        }

        if (listening)
        {
            string recognizedText = e.Result.Text;
            Console.WriteLine($"Recognized text: {recognizedText}");

            switch (recognizedText)
            {
                case "apprentice pause listening":
                    listening = false;
                    synthesizer.Speak("Listening Paused");
                    break;

                case "apprentice open youtube":
                    OpenUrl("https://www.youtube.com", "Opened Youtube");
                    break;

                case "apprentice open hollow bastion":
                    OpenUrl("https://tavern.townshiptale.com/servers/430116864/console", "Opened Hollow Bastion");
                    break;

                case "apprentice press enter":
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                    synthesizer.Speak("Pressed Enter");
                    break;

                case "apprentice open my email":
                    OpenUrl("https://mail.google.com/mail/u/0/#inbox", "Opened Email");
                    break;

                case "apprentice toggle track":
                    sim.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
                    synthesizer.Speak("Media Toggled");
                    break;

                case "apprentice toggle volume":
                    sim.Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
                    synthesizer.Speak("Audio Toggled");
                    break;

                case "apprentice volume down":
                    AdjustVolume(VirtualKeyCode.VOLUME_DOWN, "Volume turned down");
                    break;

                case "apprentice volume up":
                    AdjustVolume(VirtualKeyCode.VOLUME_UP, "Volume turned up");
                    break;

                case "apprentice skip track":
                    sim.Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
                    synthesizer.Speak("Track skipped");
                    break;

                case "apprentice previous track":
                    sim.Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
                    synthesizer.Speak("Previous track");
                    break;

                case "apprentice delete all text":
                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                    sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    synthesizer.Speak("Deleted all text");
                    break;

                case "apprentice backspace":
                    sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    synthesizer.Speak("Backspace");
                    break;

                case "apprentice command list":
                    string commandList = "apprentice open youtube\n" +
                                         "apprentice open hollow bastion\n" +
                                         "apprentice press enter\n" +
                                         "apprentice open my email\n" +
                                         "apprentice pause listening\n" +
                                         "apprentice resume listening\n" +
                                         "apprentice toggle track\n" +
                                         "apprentice volume down\n" +
                                         "apprentice volume up\n" +
                                         "apprentice skip track\n" +
                                         "apprentice previous track\n" +
                                         "apprentice delete all text\n" +
                                         "apprentice backspace\n" +
                                         "apprentice command list\n" +
                                         "apprentice toggle volume";
                    Console.WriteLine(commandList);
                    synthesizer.Speak("Command List");
                    break;
            }
        }
    }
};

void OpenUrl(string url, string response)
{
    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    synthesizer.Speak(response);
}

void AdjustVolume(VirtualKeyCode key, string response)
{
    for (int i = 0; i < 5; i++)
    {
        sim.Keyboard.KeyPress(key);
    }
    synthesizer.Speak(response);
}

=
recognizer.SetInputToDefaultAudioDevice();

recognizer.RecognizeAsync(RecognizeMode.Multiple);

Console.WriteLine("Start Speaking\nFor a List of Commands, say 'Apprentice Command List'\nTo Pause Listening, say 'Apprentice Pause Listening'\nTo Resume Listening, say 'Apprentice Resume Listening'");
Console.ReadLine();

