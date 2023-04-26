namespace DemoRichText.Model
{
    public class RichFormatFactory
    {
        public static IRichFormat CreateRichFormat(BTNType btnType)
        {
            IRichFormat richFormat;
            switch (btnType)
            {
                case BTNType.Bold:
                    richFormat = new BoldRichFormat();
                    break;
                case BTNType.BGColor:
                    richFormat = new BgColorRichFormat();
                    break;
                case BTNType.Center:
                    richFormat = new CenterRichFormat();
                    break;
                case BTNType.Del:
                    richFormat = new DelRichFormat();
                    break;
                case BTNType.Font:
                    richFormat = new FontRichFormat();
                    break;
                case BTNType.ForeColor:
                    richFormat = new ForeColorRichFormat();
                    break;
                case BTNType.FontSize:
                    richFormat = new FontSizeRichFormat();
                    break;
                case BTNType.Indent:
                    richFormat = new IndentRichFormat();
                    break;
                case BTNType.Italic:
                    richFormat = new ItalicRichFormat();
                    break;
                case BTNType.Left:
                    richFormat = new LeftRichFormat();
                    break;
                case BTNType.OutIndent:
                    richFormat = new OutIndentRichFormat();
                    break;
                case BTNType.Pic:
                    richFormat = new PicRichFormat();
                    break;
                case BTNType.Print:
                    richFormat = new PrintRichFormat();
                    break;
                case BTNType.Right:
                    richFormat = new RightRichFormat();
                    break;
                case BTNType.Search:
                    richFormat = new SearchRichFormat();
                    break;
                case BTNType.StrikeLine:
                    richFormat = new StrikeLineRichFormat();
                    break;
                case BTNType.SubScript:
                    richFormat = new SubScriptRichFormat();
                    break;
                case BTNType.SuperScript:
                    richFormat = new SuperScriptRichFormat();
                    break;
                case BTNType.Ul:
                    richFormat = new UlRichFormat();
                    break;
                case BTNType.UnderLine:
                    richFormat = new UnderLineRichFormat();
                    break;
                default:
                    richFormat = new DefaultRickFormat();
                    break;

            }
            return richFormat;
        }
    }
}
