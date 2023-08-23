# WikiDB

WikiDB is a lightweight wiki downloader meant to allow you to download, archive, and serve pages from the internet. While its original purpose is to download and sanitize pages from wikis (especially the Arch Wiki and other MediaWiki instances), it can be use to download any page from the internet.

WikiDB is written in C# / .NET Core 7. It is written exclusively for Linux, but support for Windows, MacOS, and BSD systems may come in the future. All source code is open source and licensed under GNU GPL v3.

## Usage

---

### Downloading a page

The core functionality behind WikiDB is downloading pages. There are several options behind this, but the most basic usage is as follows:

To download a page by slug from the default wiki and output to STDOUT:
```bash
~ $ wikiget <slug>
```

To download a page by slug from a specific wiki and output to a specified file:
```bash
~ $ wikiget <slug> -w <wiki> -o <file>
```

You may also download more than one page at a time by specifying more args. You may also want to index the downloaded page(s) into a database. This can be done with the `-i` flag. This will index the page(s) into the default database. You may also specify a database to index into with the `-D` flag.

WikiGet also supports recursive downloading (similar to wget's `-r` flag). This can be done with the `-r` flag. This will download the page and all pages linked to from the page. You must also specify the maximum depth of the recursion. A recursion depth of zero indicates that pages should continue to be downloaded until the bottom of the tree is reached or SIGKILL is received.

To recursively download a page and all pages linked to from the default wiki and index into database `main`:
```bash
~ $ wikiget <slug> -r 0 -iD main
```

### Indexing the Database

The database is possibly the greatest selling point behind WikiDB. The database is a simple file-system database that stores wiki pages as HTML files and keeps metadata files for each page. The file structure of a typical database might look something like this:

```plaintext
- root
  |- database.xml
  |- pages
  |  |- wiki_1
  |  |  |- page_1.html
  |  |  |- page_1.html.xml
  |  |- wiki_2
  |     |- parent_page.html
  |     |- parent_page.html.xml
  |     |- parent_page
  |        |- child_page.html
  |        |- child_page.html.xml
  |- media
     |- wiki_1
     |  |- page_1
     |     |- image_1.webp
     |     |- video_1.mp4
     |- wiki_2
        |- parent_page
           |- image_2.png
           |- image_3.png
           |- child_page
              |- image_4.png
              |- image_5.png
```

Beautiful, isn't it? The WikiDB command provides several options for interacting with the database. The most basic usage is as follows:

```bash
# Make a table of info about each registered database
~ $ wikidb
# Make a table of info about all pages in all databases
~ $ wikidb -L
# Make a table of info about all pages and wikis in a database.
~ $ wikidb -P -D main
# Provide detailed info about a page
~ $ wikidb -i page_1 -D main -W wiki_1 
```

You can also delete pages, wikis, and databases. There are some more options, but I will leave you to discover those in the help page of the program.

### Serving Pages

WikiDB comes with a custom lightweight ASP.NET Core server for serving downloaded pages locally. The server may be accessed through the wikiserve command.

```bash
# Just to serve the default database at http://localhost:80
~ $ wikiserve
# You may also specify the desired hostname and port (although I'm not sure why you would want to)
~ $ wikiserve -p 6969 -h 127.0.0.1
# You may also specify which database to serve.
~ $ wikiserve -D extra
```

## Installation

---

There are multiple options for installation, and most of them do not work yet. In the future, WikiDB will be packaged into `.deb`, `.rpm` (maybe) and `.pkg` files for easy installation with package managers. I may also write a simple install script that gets and installs the latest version on its own. For now, though, the only way to install is to build from source and manually place the necessary files where they need to go. More instructions will be provided when the application has reached a somewhat functional state.