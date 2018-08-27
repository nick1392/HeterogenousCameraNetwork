/* 
***************
* Mission.txt *
***************

* Cordinates *
x1 y1 z1 rot ct
x2 y2 z2 h2
...

where x y z are coordinates, rot is the rotation of drone (0°~360°), ct is the camera tilt (0°~90°)
the first coordinate, is where the drone start is mission!

***************
*   POI.txt   *
***************

* Coordinates *
x y z r yp v

where x y z are coordinates
r is the radius of rotation
yp is the y coord where the camera watch
v is the velocity of rotation

************************
* ParameterSetting.txt *
************************

movementForwardSpeed movementRightSpeed rotateAmountByKeys movementForwardSpeedMission rotateAmountMission modeFlag CollisionSensingDistance

where:
***IMPORTANT ALL PARAMETER ARE FLOAT (e.g. 20.0) EXCEPT modeFlag that is integer***
movementForwardSpeed->The max velocity of forward speed of drone - Default 20
movementRightSpeed->The max velocity of forward speed of drone - Default 20
rotateAmountByKeys->The max rotation speed around Yaw axis of drone - Default 2.5
movementForwardSpeedMission->The max velocity of forward speed of drone in Mission - Deafult 20
rotateAmountMission->he max rotation speed around Yaw axis of drone in Mission - Default 10
modeFlag->Starting mode of operation [0]-> Free control [1]-> Mission [2]-> POI - Default 0
CollisionSensingDistance-> how far collision sensing can be activated - Default 5


*/
