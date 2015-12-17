using Schneedetektion.Data;

namespace Schneedetektion.ImagePlayGround
{
    internal class Candidate
    {
        internal Image DifferenceImage;
        internal Image Image0;
        internal Image Image1;

        public Candidate(Image image0, Image image1, Image differenceImage)
        {
            Image0 = image0;
            Image1 = image1;
            DifferenceImage = differenceImage;
        }
    }
}