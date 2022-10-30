using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Original Author - Zach Gross

namespace DuolBots
{
    // Constants required for movement calculations
    public static class MovementConstants
    {
        public const float ACCELERATION_CONSTANT = 2000;
        // Consider decreasing this a little bit (was 65)
        public const float ROTATION_CONSTANT = 50;
        // Caps the rotation that a bot can rotate by this factor times its max power
        // Currently unused
        //public const float MAX_ROTATION_FACTOR = 0.75f;

        // Consider decreasing to allow for slightly more movement
        public const float FRICTION_CONSTANT = 4f;

        // Makes the rate that the bot loses power in air slower than normal
        public const float MIDAIR_FRICTION_FACTOR = 0.2f;

        // Consider decreasing by a substantial amount (was 2.5) and testing
        public const float GROUND_HEIGHT_THRESHOLD = 1.2f;

        // When one player is holding forward and the bot is spinning, the other play may want to hold forward
        // to even it out and start going straight. But, depending on the bot's weight, this can take quite a bit
        // of time for the new side to catch up to the old side. This would make it so that, if one wheel is moving
        // and the other wheel is not, the new side will compare between what it's power would become normally and
        // CATCHUP_FACTOR * the old side's power and take whichever has a greater magnitude (CATCHUP_FACTOR power
        // will be ignored if it is greater than the target power).
        public const float CATCHUP_FACTOR = 1; //0.4f;

        // Constants dictating how oil affects movement
        public const float OIL_MINIMUM_MUDDLE = 1.15f;
        public const float OIL_MAXIMUM_MUDDLE = 2;
        public const float OIL_MINIMUM_FRICTION = 0.3f;
        public const float OIL_RECOVERY_TIME = 3;

        // Flip mechanics
        public const float FLIP_THRESHOLD = 20;
    }
}
