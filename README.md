# MtarTool
.mtar (Motion Archive) unpacker and repacker for Metal Gear Solid V: The Phantom Pain.

Allows you to extract MGSV's animation files (.gani) from the motion archive containers.

## Usage:

Drag a .mtar file onto the tool. If it is an Mtar Type 1 file, the tool will simply extract the .gani files from it. If it is a Type 2 file, the tool will extract the .trk and .chnk files and all of the contained .gani files along
with their .exchnk and .enchnk files.

Drag a .xml file produced by the tool onto the tool to repack the .mtar.

The tool will generate a hashed_names.txt file containing any unhashed name's hashed equivalent for tracking purposes.

Set the -n parameter to add a four digit id to the beginning of every file's name. The number keeps track of the order the files were stored in the .mtar.

**Usage Example:**

> MtarTool.exe Example.mtar -n

## File Format Descriptions:

**.gani**: MGSV's animation format.

**.trk**: Description of all main animation tracks for an Mtar Type 2 file.

**.chnk**: A chunk of data with an unknown purpose belonging to an Mtar Type 2 file.

**.exchnk**: An extra chunk of animation tied to a .gani file from an Mtar Type 2 file. If the .gani file is swapped this should be brought
along with it.

**.enchnk**: A chunk of animation at the bottom of an Mtar Type 2 file. Tied to a .gani file. Should be brought along with a swapped .gani
file.

## Credits:

Thanks to id-daemon (daemon1) for giving me helpful information about the Mtar Type 2 format.
