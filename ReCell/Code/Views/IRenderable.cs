namespace Recellection.Code.Views
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework.Content;

    public interface IRenderable
	{
        #region Public Methods and Operators

        [System.Obsolete("Use Draw instead!")]
        List<DrawData> GetDrawData(ContentManager content);

        #endregion
	}
}
