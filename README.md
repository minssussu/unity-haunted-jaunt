# Assignment 2

## Description
1. [10] Get this downloaded and running:

htts://assetstore.unity.com/packages/essentials/tutorial-projects/unity-learn-3d-beginner-complete-project-urp-143846

Links to an external site.

alternatively, you can download it directly here:

https://classes.cs.uoregon.edu/25S/cs410gameprog/assignments/Haunted-Jaunt-main.zip

1. [20] Add at least one gameplay element that uses a dot product in some way (e.g., calculate length, distance, angle, facing direction).
1. [20] Add at least one gameplay element that uses linear interpolation in some way (e.g., calculate intermediate position, orientation, color).
1. [20] Add at least one new particle effect with trigger(s).
1. [20] Add at least one new sound effect with trigger(s).
1. [10] In your GitHub repo readme, describe the use of the dot product, linear interpolation, particle effect, and sound effect and how to make these happen in game. Also include the names of your team members and the contributions from each team member.


## Author

Brett DeWitt

## Solutions

### Dot Product

#### Feature

The player is alerted by a UI text message when they are facing the exit.

#### Implementation

Structurally, I tried to mimic the observer pattern used by the ghosts to detect the player. This way the project architecture is consistent, and it provides a reasonable separation of concerns. I added an `ExitDetector` object as a child to the player object `JohnLemon`. The script finds the direction of the exit relative to the player (vector subtraction), then uses Vector3.Dot to determine the players facing relative to the direction of the exit. A public `facingThreshold` value is used as a comparison to determine if the player's facing is within a reasonable margin. If it is, a UI text is enabled alerting the player that they are headed toward the exit.

```c#
    // Update is called once per frame
    void Update()
    {
        // get the direction to the exit
        Vector3 directionToExit = exit.position - transform.position;

        // use dot product to get a value indicating how closely the detector is facing the exit direction
        float facing = Vector3.Dot(transform.forward, directionToExit.normalized);

        // check if player is facing the exit within the tolerance set by the threshold
        bool isFacingExit = (facing > facingThreshold);

        // enable or disable the text based on whether the player faces the exit
        exitAlertText.enabled = isFacingExit;
    }
```
