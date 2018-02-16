# WPF Draggable Extended Window Frame Demo

A sample WPF application, written in C#, demonstrating window dragging behavior
on extended non-client window frame regions of a window in a manner similar to
desktop apps such as Internet Explorer.

The code in this sample is a companion to the following Q&A on Stack Overflow:  
[How do I make a WPF window movable by dragging the extended window frame?][1]

[1]: https://stackoverflow.com/questions/5493149/how-do-i-make-a-wpf-window-movable-by-dragging-the-extended-window-frame

## Getting the code

Clone this repository or download and extract an archive, then open it in Visual
Studio, and build and run it.

As it's a very simple demo application, it'll remain unchanged much of the time,
except for improving documentation, fixing bugs and typos, etc.

## Notes

This sample includes the necessary P/Invokes for extending non-client regions
into the client area; however their implementation is not essential to the
sample, which is only concerned with hit-tests on the frame area versus the
substitute "client area". That means you can either use my implementation or
roll your own. For details, look in the main window's code-behind file,
MainWindow.xaml.cs.

All code in this sample is distributed under the MIT license, which can be found
in [LICENSE.md](LICENSE.md).