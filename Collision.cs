namespace SpaceInvaders
{
    public class Collision
    {
       public static bool RectangleCollision(float xFirstPos
                   , float yFirstPos
                   , float firstWidth
                   , float firstHeight
                   , float xSecondPos
                   , float ySecondPos
                   , float secondWidth
                   , float secondHeight)
               {
                   return xFirstPos < (xSecondPos + secondWidth)
                          && (xFirstPos + firstWidth) > xSecondPos
                          && yFirstPos < (ySecondPos + secondHeight)
                          && (yFirstPos + firstHeight) > ySecondPos;
               }
    }
}