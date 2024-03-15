import React from 'react'

type Props = {
  leilao:any,

}


export default function LeiloesCard(props: Props) {
  return (
    <div>{props.leilao.make}</div>
  )
}
