<p align="center">
   <h1>DogHouse REST API</h1>
</p>
<p>This is a backend REST API service developed in C# using ASP.NET Core, designed to manage a simple database of dogs. The service supports operations to retrieve, add, and list dogs, with sorting and pagination features. It includes input validation and rate limiting to handle request overload scenarios.</p>

<h2>Table of Contents</h2>
<ul>
    <li><a href="#features">Features</a></li>
    <li><a href="#technologies">Technologies</a></li>
    <li><a href="#setup">Setup</a></li>
    <li><a href="#api-endpoints">API Endpoints</a></li>
</ul>

<h2 id="features">Features</h2>
<ul>
    <li><strong>Ping Endpoint:</strong> A simple health check endpoint returning API version.</li>
    <li><strong>Dog Retrieval:</strong> Retrieve a list of dogs from the database with support for sorting and pagination.</li>
    <li><strong>New Dog Entry:</strong> Allows adding new dogs to the database, with validation for unique names and data format.</li>
    <li><strong>Request Rate Limiting:</strong> Limits requests to prevent overloads, with a default of 10 requests per second.</li>
    <li><strong>Error Handling:</strong> Comprehensive validation for inputs and JSON format, including handling duplicate names, invalid data, and malformed JSON.</li>
</ul>

<h2 id="technologies">Technologies</h2>
<ul>
    <li>ASP.NET Core Web API</li>
    <li>Entity Framework Core</li>
    <li>MS SQL Server</li>
    <li>Dependency Injection</li>
    <li>Unit Testing with xUnit</li>
</ul>

<h2 id="setup">Setup</h2>

<h3>Prerequisites</h3>
<ul>
    <li><a href="https://dotnet.microsoft.com/download">.NET 8 SDK</a></li>
    <li><a href="https://www.microsoft.com/en-us/sql-server">SQL Server</a> or any compatible SQL database</li>
</ul>

<h3>Steps to Run Locally</h3>
<ol>
    <li>Clone the repository:
        <pre><code>git clone https://github.com/VladimirTkachenko3010/DogHouse
cd DogHouse</code></pre>
    </li>
    <li>Set up the database connection string in <code>appsettings.json</code>.</li>
    <li>Run migrations to initialize the database schema:
        <pre><code>dotnet ef database update</code></pre>
    </li>
    <li>Start the application:
        <pre><code>dotnet run</code></pre>
    </li>
</ol>

<h2 id="api-endpoints">API Endpoints</h2>

<h3>Ping Endpoint</h3>
<ul>
    <li><strong>GET /ping</strong>
        <p>Returns a simple version message for API health check: <code>"Dogshouseservice.Version1.0.1"</code>.</p>
    </li>
</ul>

<h3>Dog Management</h3>
<ul>
    <li><strong>GET /dogs</strong>
        <p>Retrieves a list of dogs. Supports sorting by attributes (<code>name</code>, <code>color</code>, <code>tail_length</code>, <code>weight</code>) and pagination using <code>pageNumber</code> and <code>pageSize</code> parameters.</p>
        <pre><code>curl -X GET "http://localhost/dogs?attribute=weight&order=desc&pageNumber=1&pageSize=5"</code></pre>
    </li>
    <li><strong>POST /dog</strong>
        <p>Creates a new dog entry in the database. Requires JSON data with unique <code>name</code> and valid format for <code>color</code>, <code>tail_length</code>, and <code>weight</code>.</p>
        <pre><code>{
    "name": "Doggy",
    "color": "red",
    "tail_length": 173,
    "weight": 33
}</code></pre>
    </li>
</ul>
