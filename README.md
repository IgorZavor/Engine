# Engine

<p>Simple service which are controlled by xml requests. The project includes sqlite database inside. <br>
The database includes 3 tables:</p>
<b>Users</b><br>
  <i>-id: integer</i> <br>
  <i>-Name: text</i> <br>
  <i>-Surname: text</i> <br>
  <i>-Country: text</i> <br>
  <i>-Age: integer</i> <br><br>
  
<b>Companies</b><br>
  <i>-id: integer</i> <br>
  <i>-NumberOfEmployees: integer</i> <br>
  <i>-Name: text</i> <br>
  <i>-Country: text</i> <br>
  <i>-YearFounded: integer</i> <br><br>
  
<b>Logs</b><br>
  <i>-id: integer</i> <br>
  <i>-Author: string</i> <br>
  <i>-DateTime: string</i> <br>
  <i>-Filter: text</i> <br>
  <i>-Sum: integer</i> <br><br>
<h3>METHODS:</h3>
<b><i>Generate</i></b><br><br>
// generate data for a specific table <br>
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;Generate&gt; <br>
&nbsp;&nbsp;&lt;Table&gt;Table Name&lt;/Table&gt;<br>
&nbsp;&nbsp;&lt;Count&rt;count&lt;/Count&gt;<br>
&lt;/Generate&gt;<br>
</p>
<b><i>Api:</i></b><br>
/api/engine/generate<br><br>

<i>Example:</i>
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;Generate&gt; <br>
&nbsp;&nbsp;&lt;Table&gt;Users&lt;/Table&gt;<br>
&nbsp;&nbsp;&lt;Count&rt;10&lt;/Count&gt;<br>
&lt;/Generate&gt;<br>
</p>

<b><i>Clear</i></b><br>
// clear data for a specific table
<p>
&lt;Clear&gt;<br>
&nbsp;&nbsp;&lt;Table&gt;Table Name&lt;/Table&gt;<br>
&lt;/Clear&gt;<br>
</p>
<b><i>Api:</i></b><br>
/api/engine/clear<br><br>


<i>Example:</i>
<p>
&lt;Clear&gt;<br>
&nbsp;&nbsp;&lt;Table&gt;Companies&lt;/Table&gt;<br>
&lt;/Clear&gt;<br>
</p>

<b><i>FilterBy</i></b><br>
// Set filter for specific table and write sum result in cache.
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;FilterBy&gt;<br>
&nbsp;&nbsp;&lt;Authory&gt;Author Name&lt;/Author&gt;<br>
&nbsp;&nbsp;&lt;Filters&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&lt;Filter&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;Value&gt;Filter Value 1&lt;/Value&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/Filter&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&lt;Filter&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;Value&gt;Filter Value 2&lt;/Value&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/Filter&gt;<br>
&nbsp;&nbsp;&lt;/Filters&gt;<br>
&nbsp;&nbsp;&lt;Table&gt;Table Name&lt;/Table&gt;<br>
&nbsp;&nbsp;&lt;FilterColumn>Filter Column Name&lt;/FilterColumn&gt;<br>
&nbsp;&nbsp;&lt;SummaryColumn&gt;Summary Column Name&lt;/SummaryColumn&gt;<br>
&lt;/FilterBy&gt;<br>
</p>
<b><i>Api:</i></b><br>
/api/engine/filterAndSum<br><br>


<i>Example:</i>
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;FilterBy&gt;<br>
	&lt;Authory&gt;Matt&lt;/Author&gt;<br>
	&lt;Filters&gt;<br>
		&lt;Value&gt;USA&lt;/Value&gt;<br>
	&lt;/Filters&gt;<br>
	&lt;Table&gt;Users&lt;/Table&gt;<br>
	&lt;FilterColumn>Country&lt;/FilterColumn&gt;<br>
	&lt;SummaryColumn&gt;Age&lt;/SummaryColumn&gt;<br>
&lt;/FilterBy&gt;<br>
</p>

<b><i>WriteSumToLogs</i></b><br>
<b><i>Api:</i></b><br>
api/engine/writeSumToLogs

<b><i>Entities</i></b><br>
// Get all entities from a specific table
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;Entities&gt;<br>
&nbsp;&nbsp;&lt;Table&gt;Table Name&lt;/Table&gt;<br>
&lt;/Entities&gt;<br>
</p>
<b><i>Api:</i></b><br>
/api/engine/getAllEntities<br><br>

<i>Example:</i>
<p>
&lt;?xml version="1.0" encoding="UTF-8"?&gt;<br>
&lt;Entities&gt;<br>
&nbsp;&nbsp;&lt;Table&gt;Logs&lt;/Table&gt;<br>
</p>


<h3>DOCKER IMAGE:</h3>
<i>docker pull zavordocker/engine</i>
<h3>FAST IMPLEMENTATION:</h3>
docker run -lt -p 51000:80 zavordocker/engine

<h3>HOW TO USE: </h3>
<i><b>1. PostMan:</b></i><br>
&nbsp;&nbsp;<b>1.1 Generate</b><br>
&nbsp;&nbsp;Type: Post <br>
&nbsp;&nbsp;URL: http://localhost:51000/api/engine/generate <br>
&nbsp;&nbsp;Body Type: xml <br><br>

&nbsp;&nbsp;<b>1.2 Clear</b><br>
&nbsp;&nbsp;Type: Delete <br>
&nbsp;&nbsp;URL: http://localhost:51000/api/engine/clear <br>
&nbsp;&nbsp;Body Type: xml <br>

&nbsp;&nbsp;<b>1.3 GetAllEntities</b><br>
&nbsp;&nbsp;Type: Post <br>
&nbsp;&nbsp;URL: http://localhost:51000/api/engine/getAllEntities <br>
&nbsp;&nbsp;Body Type: xml <br> <br>

&nbsp;&nbsp;<b>1.4 FilterAndSum</b><br>
&nbsp;&nbsp;Type: Post <br>
&nbsp;&nbsp;URL: http://localhost:51000/api/engine/filterAndSum <br>
&nbsp;&nbsp;Body Type: xml <br> <br>

&nbsp;&nbsp;<b>1.5 WriteSumToLogs</b><br>
&nbsp;&nbsp;Type: Post <br>
&nbsp;&nbsp;URL: http://localhost:51000/api/engine/writeSumToLogs

<i><b>2. Swagger:</b></i><br>
&nbsp;&nbsp;URL: http://localhost:51000/swagger/index.html<br>
&nbsp;&nbsp;Choose a method, that you want to test and put correspondig xml-model.
