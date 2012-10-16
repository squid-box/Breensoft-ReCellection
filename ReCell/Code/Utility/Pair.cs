namespace Recellection.Code.Utility
{
	class Pair<T>
	{
	    #region Constructors and Destructors

	    public Pair(T first, T second)
	    {
	        this.First = first;
	        this.Second = second;
	    }

	    #endregion

	    #region Public Properties

	    public T First {get; set; }
		public T Second { get; set; }

	    #endregion
	}
}
