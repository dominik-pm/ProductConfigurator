import { Typography } from '@mui/material'
import React from 'react'

export default function PriceListItem({name, price}) {
    return (
        <Typography variant="body1">
            {name}{price ? `: ${price}€` : ''}
        </Typography>
    )
}
