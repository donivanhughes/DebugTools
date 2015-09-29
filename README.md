# DebugTools
Collection of tools I built for debugging purposes. Most of them revolve around the CConsole tool.
CConsole is a Singleton class. It uses a shared instance of a WPF window, which has been set up for use in a WinForm,
as a platform to send realtime messages to. It was built to replace the windows console,
which I found both lacking in functionality, and difficult to use when using winforms.
I love the feel of the Windows Console, so it emulates it in style somewhat.
It currently has tabbing, functions to handle exceptions, and data errors. It has functions to print lists, and show all the
fields and properties, and their values, of any object(Mostly).
It has some mouse data, and memory usage for the current process.
