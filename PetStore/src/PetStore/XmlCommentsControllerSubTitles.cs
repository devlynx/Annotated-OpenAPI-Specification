using Swashbuckle.SwaggerGen.Annotations;

namespace PetStore
{
    using System;
    using System.Xml.XPath;
    using Periwinkle.Swashbuckle;

    internal class XmlCommentsControllerSubTitles : IControllerSubTitles
    {
        private readonly XPathNavigator xmlNavigator;
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private const string SubTitleXPath = "controllerSubTitle";

        public XmlCommentsControllerSubTitles()
        {
            XPathDocument xmlDoc = new XPathDocument(XmlCommentTools.GetXmlCommentsPath());
            xmlNavigator = xmlDoc.CreateNavigator();
        }

        public string GetControllerSubTitle(Type controllerType)
        {
            var commentId = XmlCommentsIdHelper.GetCommentIdForType(controllerType);
            var commentNode = xmlNavigator.SelectSingleNode(string.Format(MemberXPath, commentId));

            if (commentNode == null)
                return null;

            var subtitleNode = commentNode.SelectSingleNode(SubTitleXPath);

            if (subtitleNode == null)
                return null;

            return XmlCommentsTextHelper.Humanize(subtitleNode.InnerXml);
        }
    }
}