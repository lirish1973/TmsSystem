// Existing content...

public string GenerateHtmlContent(Model model)
{
    // Existing sections: header, customer details, trip details, guide details, price

    // Tour Description
    if (!string.IsNullOrEmpty(model.Offer.Tour.Description))
    {
        htmlContent += "<h2>אודות הסיור - " + model.Offer.Tour.Title + "</h2>";
        htmlContent += "<div>" + model.Offer.Tour.Description.Replace("\n", "<br />") + "</div>";
    }

    // Detailed Schedule
    if (model.Offer.Tour.Schedule != null && model.Offer.Tour.Schedule.Any())
    {
        htmlContent += "<h2>לוח זמנים מפורט</h2>";
        foreach (var item in model.Offer.Tour.Schedule.OrderBy(s => s.StartTime))
        {
            htmlContent += "<div class='schedule-item'>";
            htmlContent += item.StartTime + (item.EndTime != null ? " - " + item.EndTime : "") + "<br />";
            htmlContent += "<h4 class='location-title'>" + item.Location + "</h4>";
            htmlContent += "<p>" + item.Description + "</p>";
            htmlContent += "</div>";
        }
    }

    // Tour Includes
    if (model.Offer.Tour.Includes.Any())
    {
        htmlContent += "<h2>הסיור כולל</h2>";
        foreach (var include in model.Offer.Tour.Includes)
        {
            htmlContent += "<div class='include-item-tour'>" + include + "</div>";
        }
    }

    // Tour Excludes
    if (model.Offer.Tour.Excludes.Any())
    {
        htmlContent += "<h2>הסיור לא כולל</h2>";
        foreach (var exclude in model.Offer.Tour.Excludes)
        {
            htmlContent += "<div class='exclude-item-tour'>" + exclude + "</div>";
        }
    }

    // Price Includes
    if (!string.IsNullOrEmpty(model.Offer.PriceIncludes))
    {
        htmlContent += "<h2>המחיר כולל</h2>";
        var priceIncludes = model.Offer.PriceIncludes.Split('\n');
        foreach (var item in priceIncludes)
        {
            htmlContent += "<div>" + item + "</div>";
        }
    }

    // Price Excludes
    if (!string.IsNullOrEmpty(model.Offer.PriceExcludes))
    {
        htmlContent += "<h2>המחיר אינו כולל</h2>";
        var priceExcludes = model.Offer.PriceExcludes.Split('\n');
        foreach (var item in priceExcludes)
        {
            htmlContent += "<div>" + item + "</div>";
        }
    }

    // Special Requests
    if (!string.IsNullOrEmpty(model.Offer.SpecialRequests))
    {
        htmlContent += "<h2>בקשות מיוחדות וטקסים</h2>";
        htmlContent += "<div>" + model.Offer.SpecialRequests.Replace("\n", "<br />") + "</div>";
    }

    // Cancellation Terms
    htmlContent += "<h2>תנאי ביטול ותשלום</h2>";
    htmlContent += "<div class='term-item'>תשלום מקדמה</div>";
    htmlContent += "<div class='term-item'>דחיית תאריך</div>";
    htmlContent += "<div class='term-item'>ביטול עד 30 יום</div>";
    htmlContent += "<div class='term-item'>ביטול 30-14 ימים</div>";
    htmlContent += "<div class='term-item'>ביטול עד 14 ימים</div>";
    htmlContent += "<div class='term-item'>ביטול מטעמנו</div>";

    // Payment Details
    htmlContent += "<h2>פרטי תשלום</h2>";
    if (!string.IsNullOrEmpty(model.Offer.PaymentMethod))
    {
        htmlContent += "<div>שיטת תשלום: " + model.Offer.PaymentMethod + "</div>";
    }
    htmlContent += "<div>סכום לתשלום: " + model.Offer.TotalPayment + "</div>";
    htmlContent += "<h3>פרטי העברה בנקאית</h3>";
    htmlContent += "<div class='bank-details'>שם הבנק: בנק לאומי</div>";
    htmlContent += "<div class='bank-details'>מספר סניף: 805</div>";
    htmlContent += "<div class='bank-details'>מספר חשבון: 39820047</div>";
    htmlContent += "<div class='bank-details'>שם בעל החשבון: ספארי אפריקה בע"מ</div>";
    htmlContent += "<div class='bank-details'>ח.פ: 515323970</div>";
    htmlContent += "<div class='bank-details'>סכום להעברה: " + model.Offer.TotalPayment + "</div>";
    htmlContent += "<h3>פרטי קשר</h3>";
    htmlContent += "<div>טלפון: 058-7818560</div>";
    htmlContent += "<div>אימייל: info@tryit.co.il</div>";

    // Footer
    htmlContent += "<footer>...</footer>";
    return htmlContent;
}

public string GetCssStyles()
{
    return ".schedule-timeline { /* styles */ }\n" +
           ".schedule-item { /* styles */ }\n" +
           ".time-badge { /* styles */ }\n" +
           ".location-title { /* styles */ }\n" +
           ".include-item-tour { /* styles */ }\n" +
           ".exclude-item-tour { /* styles */ }\n" +
           ".term-item { /* styles */ }\n" +
           ".bank-details { /* styles */ }\n" +
           "/* Add border colors and backgrounds matching the blue theme */";
}