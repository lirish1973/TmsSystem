                var offer = await _context.Offers
                    .Include(o => o.Customer)
                    .Include(o => o.Guide)
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Schedule)
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Includes)
                    .Include(o => o.Tour)
                        .ThenInclude(t => t.Excludes)
                    .Include(o => o.PaymentMethod)
                    .FirstOrDefaultAsync(o => o.OfferId == id);