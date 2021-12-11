import React from 'react'
import { ButtonBase, Typography } from '@mui/material'
import { Box } from '@mui/system'
import { useNavigate } from 'react-router'
import './Product.css'

export default function Product({product}) {
    const navigate = useNavigate()

    const {id, name, description, image} = product
    
    function handleClick(id) {
        console.log(`clicked on: ${name} (id ${id})`)
        navigate(`/configuration/${id}`)
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
