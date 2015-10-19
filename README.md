# Hotspotizer

Hotspotizer allows users without computer programming skills to design, visualize, save and recall sets of custom full-body gestures, for use with the Microsoft Kinect. These gestures are mapped to system-wide keyboard commands which can be used to control any application.

Hotspotizer was master's research project at the [Koç University Design Lab](http://designlab.ku.edu.tr). See [mbaytas/ma-thesis](https://github.com/mbaytas/ma-thesis) for the TeX source files and PDF of the thesis.

## Links

- [Master's Thesis: End-User Authoring of Mid-Air Gestural Interactions (TeX source)](https://github.com/mbaytas/ma-thesis)
- [Koç University Design Lab](http://designlab.ku.edu.tr/)
- [Hotspotizer featured on Channel 9 Coding4Fun Kinect Projects blog](http://channel9.msdn.com/coding4fun/kinect/Todays-hot-project-Hotspotizer)

## Related Academic Publications

- Mehmet Aydın Baytaş, Yücel Yemez, and Oğuzhan Özcan (2014). Hotspotizer: end-user authoring of mid-air gestural interactions. In *Proceedings of the 8th Nordic Conference on Human-Computer Interaction (NordiCHI '14)*.
- Mehmet Aydın Baytaş, Yücel Yemez and Oğuzhan Özcan (2014). User Interface Paradigms for Visually Authoring Mid-Air Gestures: A Survey and a Provocation. In *Proceedings of the Workshop on Engineering Gestures for Multimodal Interfaces (EGMI 2014)*.

## IP stuff

The content of this repository is subject to Koç University's policies on intellectual property rights (see [here](http://gsssh.ku.edu.tr/rules-regulations) and [here](http://vprd.ku.edu.tr/research/grand)). Various bits and pieces that have somehow become part of the repository may be subject to various other policies, which should be indicated where relevant. Please observe these policies, along with some basic principles of academic honesty and common human decency, as you make use of this repository.

Conversely; if my work somehow makes use of content that belongs to you (or an organization you are affiliated with), in a way that you (or the organization you are affiliated with) do(es) not condone; please let me know, and I will do my best to observe your policies.

## Academic Project Info

**Advisors:** Oğuzhan Özcan, Yücel Yemez
**Concept Development:** Mehmet Aydın Baytaş, Ayça Ünlüer
**Software Development:** Mehmet Aydın Baytaş

Touchless full-body gesture-sensing devices such as the Microsoft Kinect have introduced a world of possibilities to interactive computing. However, the technical understanding and programming skills required to make use of these devices left them largely out of reach for designers, hobbyists, gamers and professionals outside of computing. To lower the floor of technical skills needed to utilize the technology in custom applications and to enable rapid prototyping of gesture-based interactions, we have developed Hotspotizer.

Hotspotizer allows users without computer programming skills to design, visualize, save and recall sets of custom full-body gestures. These gestures are mapped to system-wide keyboard commands which can be used to control any application that runs on the current versions of the Microsoft Windows platform.

Hotspotizer is centered around a novel, easy-to-use graphical interface, based on space discretization, for specifying free-form, full-body gestures (i.e. “teaching” gestures to a computer) for use with the Microsoft Kinect sensor.

Hotspotizer has been designed primarily for the following purposes:

- Digital arts and design education
- Rapid prototyping of full-body gesture-based interactive applications
- Gaming
- Adapting arbitrary keyboard-controlled software for gesture control

## Documentation

### Screenshots

### Requirements

- Windows 8 or Windows 8.1 recommended, also works on Windows 7
- Kinect for Windows or Kinect for XBox 360 sensor (the current version does not support Kinect for Windows v2 and Kinect for Xbox One)
- Microsoft .NET Framework 4.5\*
- Microsoft Kinect for Windows Runtime v1.8\*\*
- Microsoft Kinect for Windows SDK v1.8\*\* (Required for Kinect for XBox 360 sensors only. Not required if you have the Kinect for Windows version of the sensor.)

\*: Hotspotizer will check for and assist with the installation of these software packages during installation.

\*\*: Hotspotizer will check for and assist with the installation of these software packages during startup.

### Installation

Download the release.

Unzip the archive and run `setup.exe` to install.

Alternatively, you can directly run `Hotspotizer.exe` under `Application Files/Hotspotizer_X_X_X_X/`.

### Getting Started

Hotspotizer consists of three modules:

- The Manager module is for managing gesture sets: Saving and loading sets of gestures; removing gestures from a set; launching the Editor to make changes to a gesture or to add a new gesture to the set; and launching the Visualizer to begin keyboard emulation.
- The Editor module is for creating new gestures or making changes on an existing gesture.
- The Visualizer module performs keyboard emulation and lets the user see if they are executing their gestures correctly.

Start Hotspotizer to launch a Manager screen with an empty gesture set:

*If Hotspotizer can’t successfully connect to your Kinect sensor, a warning will be displayed on the right side and Kinect-related features will be disabled.*

Click the “Add New Gesture” button to launch the Editor.

We’ll implement a simple “swipe” gesture with the right hand, from the left to the right. We’ll map this gesture to the “space” key.

After naming the gesture, click the “Keyboard Command” button to set the keyboard mapping. Press the “space” button and click the “check” symbol to accept.

*The “Press/Hold” toggle button next to the “Keyboard Command” button sets whether performing the gesture should trigger a single key press or hold down the keyboard button as long as the tracked limb remains in the hotspots of the gesture’s last frame.*

*You can map your gestures to any combination of keys, not just a single key.*

Select the limb that will be used to perform the gesture – in this case, the right hand.

We’ll implement a “swipe” gesture that incorporates 3 frames: The right hand will pass through “hotspots” on the left, in the middle, and on the right, respectively.

Mark the hotspots for the first frame on the front view grid first. On the side view grid, which is completely disabled to start with, this will enable the rows that correspond to those hotspots. Then mark hotspots on the side view grid. You will see the hotspots visualized in the 3D viewport.

*A “hotspot” denotes a 15 cm x 15 cm x 15 cm cubic cell. The front view grid marks where these cells lie horizontally and vertically. The side view grid marks where they lie vertically and depth-wise. Hotspotizer tracks the selected limbs for each gesture and registers when they pass through the marked hotspots.*

*You can rotate the 3D viewport in the Editor and the Visualizer by right-clicking it and holding the right mouse button as you move the mouse around. You can zoom in and out using the mouse wheel or ctrl + right mouse button.*

Step in front of the Kinect to try them out. See if the hotspots you marked really correspond to where you want your right hand to be at when you begin the gesture.

Add the second (middle) frame using the “Add New Frame” button and mark hotspots on it. Then add the third frame.

*You can go back and edit frames by clicking on them on the timeline at the bottom. You can also re-order and delete frames using the buttons below the frame’s mini-grids.*

*As you add frames, hotspots of the previous frames will still appear on the 3D viewport, but they will become more transparent. If there are other gestures in the gesture set you are working on, those will also appear there, and they will be very transparent (see screenshots above). This is to aid the user in keeping track of where their current hotspots are in relation to the other gestures in the set.*

Save the gesture. The gesture is now part of the gesture collection loaded in the Manager and you can see its press/hold behavior, keyboard mapping and name listed in there, along with buttons to launch the Editor to make changes on the gesture and delete the gesture from the gesture collection completely.

Click the “Begin Emulation” (or “Play”) button to launch the visualizer. The visualizer will display:

- A color-coded list of the gestures in the current gesture collection with their press/hold behavior, key mapping and name.
- On a zoomable and rotatable 3D viewport and non-movable front- and side-view ports; the hotspots, color-coded, of all of the gestures in your gesture collection. The opacity of the hotspots corresponds to which frame they belong to within the gesture. Transparent hotspots are to be touched before more opaque hotspots.

The hotspots and the list items will light up when you perform the gesture. Step in front of the Kinect and try it!

When a tracked limb passes through the hotspots that belong to consecutive frames of a gesture, that gesture’s keyboard command is triggered. This also works when Hotspotizer is minimized or when another application is in use.

This “swipe” gesture, for instance, may be used now to advance slides while giving a presentation.

In the manager, you can add new gestures to your gesture collection (for example, a “reverse swipe” gesture mapped to the “backspace” key to display the previous slide while giving a presentation), or save the gesture collection for later use.

*Gesture collections are saved as “JSON” data, with the extension “.hsjson”. Curious and technically adept users may edit those files to explore possibilities.*

### Release Notes

- 1.0.0 introduces updates to the GUI, along with performance and stability improvements.
- 0.9.7 introduces a fix that prevents a gesture from having no frames.
- 0.9.6 introduces changes to the UI and fixes a bug in keyboard emulation which affected gestures mapped to combinations of keys.
- 0.9.5 fixes an issue with the keyboard key capture that disallowed key combinations involving the “alt” key; and introduces minor user interface tweaks on the Visualizer.
- 0.9.4 introduces tweaks on the user interface to improve hotspot legibility in the Editor.
- 0.9.3 fixes an issue with the gesture recognizer that caused single-frame gestures configured to relay single key presses to hold down keys.
- 0.9.2 fixes the problem of icons not being displayed correctly on Windows 7.
- 0.9.1 fixes minor issues in gesture recognizer.
- 0.9.0 initial public release.

### Known Issues in the Current Version

- The application may crash ungracefully upon startup, after displaying a warning message, if the Kinect Runtime is not installed.
