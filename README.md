# UnityTransformToolkit

*If you've found this project helpful or useful then please think about donating to buy me a cup of tea and continue my work on projects such as this one.*

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://paypal.me/joelrlb)

A custom transform toolkit made by Joel.   
When selecting any object, the selection order *DOES* matter. There is a handy little button called "Select All Children In Hierarchy" which will select the child objects in the parent selected in the order that it sits in the inspector hierarchy.   
   
Although most of the examples here only show the transformation of GameObjects being manipulated, it is worth nothing that this toolkit also allows for Rotation manipulation. All rotation for every attribute within the tool kit will use local rotation and position of the object and not the global rotation or position.
   
### Move   
With the toolkit "Active" you are able to move each item within the selection individually. If you have a selection and Disable the toolkit then you are able to move the entire group together.   
   
<p align="center" style="background-color: #383838;">
  <img src="Readme_Assets/Move.gif"/ >
</p>
   
### Spread   
There are 4 different spread options to choose from. All spreads work with both even and odd selections.   

#### Left Corner   
CornerL will allow you to pull the left corner, with the right corner pinned.   
<p align="center" style="background-color: #383838;">
  <img src="Readme_Assets/CornerLeft.gif"/ >
</p>
   
#### Right Corner
CornerR will allow you to pull the right corner, with the left corner pinned.   
<p align="center" style="background-color: #383838;">
  <img src="Readme_Assets/CornerRight.gif"/ >
</p>
   
#### Middle   
Middle will allow you to pull the middle, with the outer corners pinned.   
<p align="center" style="background-color: #383838;">
  <img src="Readme_Assets/Middle.gif"/ >
</p>
   
#### Edge   
Edge will allow you to pull the outer corners, with the middle pinned.   
<p align="center" style="background-color: #383838;">
  <img src="Readme_Assets/Edge.gif"/ >
</p>
   
### Distribute   
The distribute functions will distribute between the first and last items in the list. A purple line will be shown from the first to last object to help you see in which direction the items will be distrubted.
#### Across Line   
##### Axis Control   
You are able to distribute your selection evenly along any axis (position and rotation): X, Y, Z, or all three.   
<p align="left" style="background-color: #383838;">
  <img src="Readme_Assets/DistubX.gif"/ >
</p>

<p align="left" style="background-color: #383838;">
  <img src="Readme_Assets/DistubY.gif"/ >
</p>
   
#### Across Area

### Selection   
You have 5 buttons which allow you to scroll through a selection. This is handy when paired to the move tool. Each item within a selection has a number by it to show you where abouts it is in the list of items that you have selected.
<p align="left" style="background-color: #383838;">
  <img src="Readme_Assets/Selection.gif"/ >
</p>

### Circular Spread   
This will spread the selected objects evenly around the projected disc. When you use this mode, the last object in the selection will generate a disc near it. You can use the disc size slider within the GUI to change the radius of the disc. You will see yellow dots which will show you where the objects in your selection will appear. All objects will be rotated to look towards the centre of the disc.
<p align="left" style="background-color: #383838;">
  <img src="Readme_Assets/circleDemo.gif"/ >
</p>



## To Add

* Distribute across 2 point area
* Save custom user defined selections
* Quick Selection tools (grouping, offset - I.e. select 2 gameobjects in selection every 5 objects)
* Add curves 
* Get Spread on circle working properly
