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

## Author

Ashley Rush

### Linear Interpolation

#### Feature

If the player comes into contact with a bed, there is a 1 in 10 chance they will be sent to the end. Otherwise, they will rise in the air for three seconds, and then be teleported back to the beginning.

#### Implementation

Each fixed update checks to see if the player has recently collided with a bed and did not get the 1 in 10 chance to go to the end. If this condition is met, a timer will reset, and the player will smoothly rise a bit into the air each frame. The distance to rise each frame is determined via linear interpolation, with the parameters of the player's original position, the position in the air (which is the player's original position, but with 10 added to the Y-value), and the time elapsed divided by three seconds (the time to spend rising into the air).

```c#
void FixedUpdate()
{
    if (rand_bed_action >= 0)
    {
        if (time_spent_rising >= time_to_spend_rising)
        {
            this.transform.position = new Vector3(-9.8f, 0.0f, -3.2f);
            last_bed.SetActive(true);
            rand_bed_action = -1;
        }
        else
        {
            //add time since last frame to the timer
            time_spent_rising += Time.deltaTime;
            //set player position to position in the air. resembles the "my people need me" meme
            this.transform.position = Vector3.Lerp(original_player_pos, rise_to_pos, time_spent_rising / time_to_spend_rising);
        }
    }
    else
	//normal Fixed_Update execution

```

```c#
void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Bed"))
    {
        //1 in 10 chance of being sent to the end, otherwise rise in the air and teleport to the start
        // < insert "my people need me" meme here >
        rand_bed_action = Random.Range(0, 11);

        if (rand_bed_action == 10)
        {
            //send player to the end
            rand_bed_action = -1;
            this.transform.position = new Vector3(18, 0, 2);
        }
        else
        {
            last_bed = other.gameObject;
            last_bed.SetActive(false);
            original_player_pos = this.transform.position;
            rise_to_pos = original_player_pos;
            rise_to_pos.y += 10;
            time_spent_rising = 0;
        }
    }
}
```

## Author

Abhinav Palacharla

### Sound Effect Triggers

#### Feature

Sound Effects Implemented:

1. **Bed Interaction Sounds**: When the player interacts with beds, sounds play based on the outcome (successful teleport to end or rising/teleporting to start)
2. **Ghost Proximity Warning**: Warning sounds play when the player gets close to ghosts but hasn't been detected yet
3. **Floor Creak Sounds**: Certain floor areas make creaking sounds when the player walks over them

#### Implementation

```c#
if (rand_bed_action == 10)
{
    // Play the success teleport sound
    if (m_TeleportSuccessSound != null)
    {
        m_BedAudioSource.clip = m_TeleportSuccessSound;
        m_BedAudioSource.Play();
    }

    //send player to the end
    rand_bed_action = -1;
    this.transform.position = new Vector3(18, 0, 2);
}
else
{
    // Play the failure/rise sound
    if (m_TeleportFailSound != null)
    {
        m_BedAudioSource.clip = m_TeleportFailSound;
        m_BedAudioSource.Play();
    }

    // Rest of the bed teleportation code...
}
```

For ghost proximity warnings, a new component monitors the distance between the player and ghosts:

```c#
void Update()
{
    // Calculate distance to player
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    // If in warning distance and enough time has passed since last warning
    if (distanceToPlayer <= warningDistance &&
        Time.time - m_LastWarningTime > m_WarningCooldown)
    {
        // Play warning sound
        if (m_AudioSource != null && m_AudioSource.clip != null && !m_AudioSource.isPlaying)
        {
            m_AudioSource.Play();
            m_LastWarningTime = Time.time;
        }
    }
}
```
