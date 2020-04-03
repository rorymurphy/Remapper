# Remapper

Remapper is designed to enable simplicty in design of API entities while retaining the performance that comes from having queries run at the optimal location (often the database). Often, a mismatch between the entities we want to expose and it's persisted form, this results in a second "data object" class.

However, this comes with the significant limitation that the application must statically map API calls to well-defined queries against the data object. Because the data model knows nothing about the API entity, we lose one of the most powerful features of LINQ & the Entity Framework - the ability to take an arbitrary query against an IQueryable and execute it where it will perform best. Remapper solves this problem by providing a generic translator that rewrites the query's expression tree and passes the translated tree through to a backing IQueryProvider (e.g. a Database connector).

The code below demonstrates the sort of mappings that are possible. Here we have a list of ContentItem objects, however we want to query against them as though they are BlogPosts (where a BlogPost may be a subclass of ContentItem, but need not be). As the assertions demonstrate, the library successfully translates the query, retrieves the results, and uses the transformation function to create BlogPost objects only for the selected items.

```
var data = new List<ContentItem>();
for(int i = 0; i < 100; i++)
{
	data.Add(new ContentItem(new Dictionary<string, object>() { { "blog-content", "text" + i } }) { Title = "Blog Post " + i, Id = Guid.NewGuid() });
}

var provider = new TransformQueryProvider<ContentItem, BlogPost>()
{
	InnerQueryable = data.AsQueryable(),
	Transform = ci => new BlogPost() { Id = ci.Id, Title = ci.Title, Content = (string)ci.Attributes["blog-content"] }
};

provider.AddMapping(b => b.Content, ci => (string)ci.Attributes["blog-content"]);

var transformed = provider.CreateEmptyQuery();

//Tests translation of a property and a method call
Assert.Equal("Blog Post 50", transformed.Single(bp => !string.IsNullOrEmpty(bp.Content) && bp.Content.Equals("text50")).Title);
Assert.Equal(100, transformed.Count());
```