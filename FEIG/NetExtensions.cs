namespace NetExtensions
{
	public static class Helpers
	{
		/// <summary>
		/// Swaps the values of two objects
		/// </summary>
		public static void Swap<T>(ref T A, ref T B)
		{
			T temp = A;
			A = B;
			B = temp;
		}
	}
}
