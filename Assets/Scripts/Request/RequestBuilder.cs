using System;

namespace AntCore.Social
{
    public static class RequestBuilder
    {
        public static string GetCategories(CategoryType categoryType)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryManagers.php?f=GetCategoriesOnlyType&type={1}",
                RequestPath.serverURL, categoryType);
        }

        public static string GetGroups(string category)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryManagers.php?f=GetCategoriesOnParentCategory&category={1}",
                RequestPath.serverURL, category);
        }

        /*public static string GetGroups(string category)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryManagers.php?f=GetCategoriesOnParentCategory&category={1}",
                RequestPath.serverURL, category);
        }*/

        public static string UploadThumbnail(string path)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=UploadThumbnail&path={1}", RequestPath.serverURL, path);
        }

        public static string AddCategory(string category,CategoryType categoryType)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=AddCategory&category={1}&categoryType={2}", RequestPath.serverURL, category, categoryType.ToString());
        }

        public static string IsExistCategory(string category)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=ExistCategory&category={1}", RequestPath.serverURL, category);
        }

        public static string CreateCategory(string category,string translateBody)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=CreateCategory&translateBody={1}&category={2}", RequestPath.serverURL, translateBody, category);
        }

        public static string IsExistSubCategory(string category,string subCategory)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=ExistSubCategory&category={1}&subCategory={2}", RequestPath.serverURL, category, subCategory);
        }

        public static string AddSubCategory(string nameSubCategory, string translateNameSubCategory,string category)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryCreator.php?f=AddSubCategory&name={1}&translateName={2}&category={3}", RequestPath.serverURL,
                nameSubCategory, translateNameSubCategory, category);
        }

        public static string GetSubCategories(string category)
        {
            return string.Format("{0}applications/EnglishFacts/CategoryManagers.php?f=GetSubCategories&category={1}",
                RequestPath.serverURL, category);
        }

        public static string IsExistGroup(string category, string group)
        {
            return string.Format("{0}applications/EnglishFacts/GroupCreator.php?f=ExistGroup&category={1}&group={2}", RequestPath.serverURL, category, group);
        }

        public static string CreateGroup(string category, string group)
        {
            return string.Format("{0}applications/EnglishFacts/GroupCreator.php?f=CreateGroup&category={1}&group={2}", RequestPath.serverURL, category, group);
        }

        public static string CreateTranslateInfoGroup(string body, string category, string group)
        {
            return string.Format("{0}applications/EnglishFacts/GroupCreator.php?f=CreateTranslate&body={1}&category={2}&group={3}", RequestPath.serverURL,body, category, group);
        }

        public static string CreateAuthorsGroup(string body, string category, string group)
        {
            return string.Format("{0}applications/EnglishFacts/GroupCreator.php?f=CreateAuthors&body={1}&category={2}&group={3}", RequestPath.serverURL, body, category, group);
        }

        public static string AddGroupInSubCategory(string category,string subcategory, string group)
        {
            return string.Format("{0}applications/EnglishFacts/GroupCreator.php?f=AddGroupInSubCategory&category={1}&subcategory={2}&group={3}", RequestPath.serverURL,category, subcategory, group);
        }
    }
}