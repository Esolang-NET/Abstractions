namespace Esolang.Processor;

/// <summary>
/// Represents an I/O event.
/// </summary>
public abstract class IOEvent
{
    IOEvent() { }

    /// <summary>
    /// Creates an event requesting a character input.
    /// </summary>
    /// <param name="write">The action to write the input character to the processor.</param>
    /// <returns>An event requesting a character input.</returns>
    public static InputCharEvent InputChar(Action<char> write) => new(write);

    /// <summary>
    /// Creates an event requesting an integer input.
    /// </summary>
    /// <param name="write">The action to write the input integer to the processor.</param>
    /// <returns>An event requesting an integer input.</returns>
    public static InputIntEvent InputInt(Action<int> write) => new(write);

    /// <summary>
    /// Creates an event requesting a string line input.
    /// </summary>
    /// <param name="write">The action to write the input string line to the processor.</param>
    /// <returns>An event requesting a string line input.</returns>
    public static InputLineEvent InputLine(Action<string> write) => new(write);

    /// <summary>
    /// Creates an event requesting a string input.
    /// </summary>
    /// <param name="write">The action to write the input string to the processor.</param>
    /// <returns>An event requesting a string input.</returns>
    public static InputStringEvent InputString(Action<string> write) => new(write);

    /// <summary>
    /// Creates an event that outputs a character.
    /// </summary>
    /// <param name="output">The character to output.</param>
    /// <returns>An event that outputs a character.</returns>
    public static OutputCharEvent OutputChar(char output) => new(output);

    /// <summary>
    /// Creates an event that outputs an integer.
    /// </summary>
    /// <param name="output">The integer to output.</param>
    /// <returns>An event that outputs an integer.</returns>
    public static OutputIntEvent OutputInt(int output) => new(output);

    /// <summary>
    /// Creates an event that outputs a string line.
    /// </summary>
    /// <param name="output">The string line to output.</param>
    /// <returns>An event that outputs a string line.</returns>
    public static OutputLineEvent OutputLine(string output) => new(output);

    /// <summary>
    /// Creates an event that outputs a string.
    /// </summary>
    /// <param name="output">The string to output.</param>
    /// <returns>An event that outputs a string.</returns>
    public static OutputStringEvent OutputString(string output) => new(output);

    /// <summary>
    /// Creates an event indicating the end of execution.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    /// <returns>An event indicating the end of execution.</returns>
    public static EndEvent End(int exitCode) => new(exitCode);

    /// <summary>
    /// Represents an event requesting a character input.
    /// </summary>
    public sealed class InputCharEvent(Action<char> write) : IOEvent
    {
        /// <summary>
        /// Writes the input character to the processor.
        /// </summary>
        /// <param name="c">The input character.</param>
        public void Write(char c) => write(c);
    }

    /// <summary>
    /// Represents an event requesting an integer input.
    /// </summary>
    /// <param name="write">The action to write the input integer to the processor.</param>
    public sealed class InputIntEvent(Action<int> write) : IOEvent
    {
        /// <summary>
        /// Writes the input integer to the processor.
        /// </summary>
        /// <param name="i">The input integer.</param>
        public void Write(int i) => write(i);
    }

    /// <summary>
    /// Represents an event requesting a string line input.
    /// </summary>
    /// <param name="write">The action to write the string line to the processor.</param>
    public sealed class InputLineEvent(Action<string> write) : IOEvent
    {
        /// <summary>
        /// Write the input string line to the processor.
        /// </summary>
        /// <param name="l">The input line string.</param>
        public void Write(string l) => write(l);
    }

    /// <summary>
    /// Represents an event requesting a string input.
    /// </summary>
    /// <param name="write">The action to write the string to the processor.</param>
    public sealed class InputStringEvent(Action<string> write) : IOEvent
    {
        /// <summary>
        /// Write the input string to the processor.
        /// </summary>
        /// <param name="s">The input string.</param>
        public void Write(string s) => write(s);
    }



    /// <summary>
    /// Represents an event that outputs a character.
    /// </summary>
    /// <param name="Output">The character to output.</param>
    public sealed class OutputCharEvent(char Output) : IOEvent
    {
        /// <summary>
        /// The character to output.
        /// </summary>
        public char Output { get; } = Output;
    }

    /// <summary>
    /// Represents an event that outputs an integer.
    /// </summary>
    /// <param name="Output">The integer to output.</param>
    public sealed class OutputIntEvent(int Output) : IOEvent
    {
        /// <summary>
        /// The integer to output.
        /// </summary>
        public int Output { get; } = Output;
    }

    /// <summary>
    /// Represents an event that outputs a string line.
    /// </summary>
    /// <param name="Output">The string line to output.</param>
    public sealed class OutputLineEvent(string Output) : IOEvent
    {
        /// <summary>
        /// The string line to output.
        /// </summary>
        public string Output { get; } = Output;
    }

    /// <summary>
    /// Represents an event that outputs a string.
    /// </summary>
    /// <param name="Output">The string to output.</param>
    public sealed class OutputStringEvent(string Output) : IOEvent
    {
        /// <summary>
        /// The string to output.
        /// </summary>
        public string Output { get; } = Output;
    }

    /// <summary>
    /// Represents an event indicating the end of execution.
    /// </summary>
    /// <param name="exitCode">The exit code.</param>
    public sealed class EndEvent(int exitCode) : IOEvent
    {
        /// <summary>
        /// The exit code.
        /// </summary>
        public int ExitCode { get; } = exitCode;
    }

}
