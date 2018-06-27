
namespace WesternSpace.Utility
{
    /// <summary>
    /// This categorizes the the type of damage that happens in the world.
    /// This is needed by the collision framework to check and see if a collision
    /// matters or not.
    /// </summary>
    public enum DamageCategory
    {
        /// <summary>
        /// The Player category. 
        /// </summary>
        Player,
        
        /// <summary>
        /// The Enemy category.
        /// </summary>
        Enemy,

        /// <summary>
        /// Anything.
        /// </summary>
        All
    }
}