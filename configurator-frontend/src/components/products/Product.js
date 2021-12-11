import React from 'react'
import { ButtonBase, Typography } from '@mui/material'
import { Box } from '@mui/system'

export default function Product({product}) {
    const {id, name, description, image} = product
    
    function handleClick(id) {
        console.log(`clicked on ${id}`)
    }

    return (
        <ButtonBase className="Product" onClick={() => handleClick(id)}>
            <Box>
                <Typography variant="h2" color="main">{name}</Typography>
                <Typography>{description}</Typography>
            </Box>
        </ButtonBase>
    )
}
