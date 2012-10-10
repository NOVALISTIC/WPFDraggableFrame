WPF Draggable Extended Window Frame Demo
http://wpfdraggableframe.codeplex.com

Project Description
-------------------
A sample WPF application, written in C#, demonstrating window dragging behavior on
extended non-client window frame regions of a window, similarly to desktop apps such
as Internet Explorer.

The code in this project is a companion to the following Q&A on Stack Overflow:
http://stackoverflow.com/questions/5493149/how-do-i-make-a-wpf-window-movable-by-dragging-the-extended-window-frame

Notes
-----
The sample includes the necessary P/Invokes for extending non-client regions into the
client area; however their implementation is not essential to the sample, which is
only concerned with hit-tests on the frame area versus the substitute "client area".
That means you can either use my implementation or roll your own. For details, look
in the main window's code-behind file, MainWindow.xaml.cs.

All code in this sample project is distributed under the MIT license, which can be
found in license.txt.