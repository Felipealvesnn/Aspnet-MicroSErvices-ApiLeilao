import React from 'react'
import LeiloesCard from './LeiloesCard';

async function fetchListings() {
  const res = await fetch('http://localhost:7002/api/search')
  if (!res.ok) {
    throw new Error('Failed to fetch listings')
  }
  return res.json();
}


export default async function Listings() {
  const listings = await fetchListings()
  return (
    <div>
      {listings && listings.result.map((leilao: any) => (
        <LeiloesCard leilao={leilao} />
      )
      )}

    </div>
  )
}
