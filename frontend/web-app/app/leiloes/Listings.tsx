import React from 'react'

async function fetchListings() {
  const res = await fetch('http://localhost:6001/search')
  if (!res.ok) {
    throw new Error('Failed to fetch listings')
  }
  return res.json();
}


export default async function Listings() {
    const listings = await fetchListings()
  return (
    <div>
     {JSON.stringify(listings, null, 2)}

    </div>
  )
}
