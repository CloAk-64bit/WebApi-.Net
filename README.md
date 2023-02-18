# WebApi-.Net

# What does this API DO?
This code defines a class PropertyController that has several methods. It uses the HtmlAgilityPack library to extract data from a website and returns a list of PropertyModel objects. The GetDataFromWebsiteAsync method extracts data from a single page, while the ScrapeWebsiteAsync method extracts data from multiple pages. The GetData method is an HTTP GET endpoint that returns the extracted data as JSON. The ILogger is used for logging purposes, while the IHttpClientFactory is used to create and manage instances of HttpClient.
