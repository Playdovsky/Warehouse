namespace Unit_testing
{
    public class PhoneNumberTests
    {
        [Fact]
        public void GoodNumber1()
        {
            string phoneNumber = "752-634-879";

            Assert.True(Check(phoneNumber));
        }

        [Fact]
        public void GoodNumber2()
        {
            string phoneNumber = "655 438 671";

            Assert.True(Check(phoneNumber));
        }

        [Fact]
        public void GoodNumber3()
        {
            string phoneNumber = "512733775";

            Assert.True(Check(phoneNumber));
        }

        [Fact]
        public void BadNumber1()
        {
            string phoneNumber = "74256762";

            Assert.False(Check(phoneNumber));
        }

        [Fact]
        public void BadNumber2()
        {
            string phoneNumber = "678 723 3s1";

            Assert.False(Check(phoneNumber));
        }

        [Fact]
        public void BadNumber3()
        {
            string phoneNumber = "777-642-5618";

            Assert.False(Check(phoneNumber));
        }

        private static bool Check(string phoneNumber)
        {
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            if (phoneNumber.Length == 9 && phoneNumber.All(Char.IsDigit))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}