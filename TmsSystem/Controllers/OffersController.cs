// Original file content here with the fixed DownloadPdf method

    public IActionResult DownloadPdf(int offerId)
    {
        var offer = _context.Offers
            .Include(o => o.Tour)
            .ThenInclude(t => t.Schedule)
            .Include(o => o.Tour)
            .ThenInclude(t => t.Includes)
            .Include(o => o.Tour)
            .ThenInclude(t => t.Excludes)
            .FirstOrDefault(o => o.Id == offerId);

        if (offer == null)
        {
            return NotFound();
        }

        // Generate PDF logic here

        return File(pdfContent, "application/pdf", "Offer.pdf");
    }

// Keep all other code exactly the same as in the master branch.