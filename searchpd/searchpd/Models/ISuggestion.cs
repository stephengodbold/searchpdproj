namespace searchpd.Models
{
    public interface ISuggestion
    {
        /// <summary>
        /// Name used for sorting this suggestion
        /// </summary>
        string SortedName { get; }

        /// <summary>
        /// Returns the html representation of a suggestion
        /// </summary>
        /// <param name="subString">
        /// Sub string to highlight in the html. Pass null to not hightlight anything.
        /// </param>
        /// <returns></returns>
        string ToHtml(string subString);
    }
}
