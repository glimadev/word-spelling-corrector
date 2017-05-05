using System;

namespace SpellingCorrector
{
    class Program
    {
        static void Main(string[] args)
        {
            SpellingCorrector spellingCorrector = new SpellingCorrector();

            while (true)
            {
                Console.WriteLine("Digite uma palavra :");

                string spelling = Console.ReadLine();

                Console.WriteLine(string.Format("Você quis dizer {0} ?", spellingCorrector.Correction(spelling)));

                Console.WriteLine(spellingCorrector.GetCandidates());
            }
        }
    }
}
