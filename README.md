# Remapper

Remapper is designed to enable simplicty in design of API entities while retaining the performance that comes from having queries run at the optimal location (often the database).
Often, a mismatch between the entities we want to expose and it's persisted form, this results in a second "data object" class. However, this comes with the significant limitation
that the application must statically map API calls to well-defined queries against the data object. Because the data model knows nothing about the API entity,
we lose one of the most powerful features of LINQ & the Entity Framework - the ability to take an arbitrary query against an IQueryable and execute it where it will perform best.
Remapper solves this problem by providing a generic translator that rewrites the query's expression tree and passes the translated tree through to a backing IQueryProvider
(e.g. a Database connector).