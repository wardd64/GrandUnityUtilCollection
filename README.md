# Grand Unity Utility Collection
Collection of publicly available, c# Utility functions for Unity3D. 

--------------------------

# Introduction

The aim of the GUUC is to independently add functionality to Unity that should really have been included in the default product. Saving, Loading, Compressing, Math, Physics, manipulating Vectors, Quaternions, Transforms and much, much more.<br/>
GUUC will provide such functions while not being as intrusive as Unity frameworks and typical closed-source asset packs. In the mean-time, any fancy utility functions you made for your games in the past can find a new life in new games as long as you're willing to share!<br/>
Anyone can contribute to GUUC, with the community working together we will make the lives of Unity devs around the world a bit easier.

# Making use of GUUC

GUUC scripting is contained within the 'GUC' namespace in order to ensure compatibility with existing Unity projects. In the simplest case you can simply download all the files and insert them into the assets folder of your Unity project and off you go.<br/>
If you are using Git for your project anyway, it might be more useful to add GUC as a submodule. That way, you can easily pull new updates and push contributions on your branch.

# Contributing to GUUC

GUUC is supposed to be completely open source, so anyone can contribute. We do want to uphold some level of quality control for the final product, so make sure to read over the guidelines first. If nothing else, **look out for any pieces of text in bold**. To upload your contributions simply clone the repo into the Asset folder of a Unity project, or include it as a submodule. Push changes to a new branch and get going!

## Filtering
Not every piece of c# code -no matter how well written- is fit for the GUUC. Most importantly, the function you want to add needs be something general and flexible so that it can be used in a wide variety of Unity projects. It must be independant of the structure of the project where it comes from. In other words, each contribution should be a proper 'Utility' type function. Contributions may also include very general Monobehaviour, Editor and shader scripts.<br/>
GUUC is not a place for prefabs or assets. If you want to contribute these to the Unity community, make a free asset for them on the asset store.

## Cross dependencies
Code may only rely on other functions in the same contribution, or functions on the master branch. No external downloads should ever be necessary.

## Duplicates
In order to keep some oversight, **avoid adding duplicates**. If you want to add a function that is slightly different than an existing one, you should typically just expand that function, with an (optional) parameter to include your use case. If you absolutely must add a new function, make sure that the name of it alone clearly distinghuises what it does and what it does differently.

## Coding quality
The code will surely turn into a big fat mess over time, even so, do your best to postpone the inevitable. Never write duplicate code, add formal documentation (///), always document your parameters, come up with names that are short and to the point, but still clear, watch out for performance when dealing with nested loops, avoid messy if/else trees or redundant switch/case blocks, keep methods short, around ~20 lines before you should split it up into (private) subfunctions.

## Classifications
Currently, the following subclassifications exist. There is some overlap between these, so do not feel too strictly about how to classify your contribution. In some cases it may be preferrable to add your function to multiple functions.

### Static Utility Functions
The most pure of Utility, working entirely in static classes. New classes may be added as needed, current classes are:

| Class    | Description                    |
| --------:| ------------------------------:|
| Anim     | Animation and Animator related |
| Bits     | Binary IO |
| Cam      | 3D Camera related |
| Colrs    | Colors |
| Compr    | Data Compression |
| Files    | File IO |
| Lerp     | Interpolation and smoothing (not necessarily linear) |
| Math     | Pure floating point and integer math |
| Meshs    | Generating or analyzing meshes |
| Phys     | Unity Physics engine related |
| Quat     | Rotations |
| Rand     | Random number / object generation |
| Rects    | Rect transforms, Rects, 2D transforms |
| Text     | Text processing and strings |
| Txtur    | Texture processing, 2D or render |
| Trnsf    | 3D Transforms, Unity parent/Child, component etc. |
| Vect     | Manipulating vectors of any dimension |
| Varia    | All the rest |

### Components
Monobehaviour scripts that perform a simple and generally useful function.

### EditorComponents
Monobehaviour scripts that have a use in the Unity Editor<br/>
**Important Note** Make sure that editor scripts can still compile freely!! The usual strategy here is to make the script itself empty and add the actual editor functionality inside a conditional compile block (#if UNITY_EDITOR ... #endif)

### Helper classes
Classes that do not implement Monobehaviour, but are rather meant useful to gather common data and functions in other classes.

### Serialization
Data classes that can be used to save and load scene states

### Shaders
Shaders that have a simple function to the point where you wonder why it's not in Unity by default.

## Various Notes
Line endings are Windows (CR LF) by default<br/>
Use extension methods whenever sensible.<br/>
For now we are not comitting meta files. These are already .git-ignored.<br/>

# Credits
Guuc, it does not suuc - WARdd
