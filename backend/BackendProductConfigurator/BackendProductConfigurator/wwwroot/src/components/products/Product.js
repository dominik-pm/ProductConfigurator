import React from 'react'
import { ImageListItem, ImageListItemBar } from '@mui/material'
import { useNavigate } from 'react-router'
import { extractDescriptionFromProduct, extractIdFromProduct, extractImagesFromProduct, extractNameFromProduct } from '../../state/product/productSelector'
import { getImageSource } from '../../App'
import { baseURL } from '../../api/general'

export default function Product({ product }) {
    const navigate = useNavigate()

    const configId = extractIdFromProduct(product)
    const name = extractNameFromProduct(product)
    const description = extractDescriptionFromProduct(product)
    const images = extractImagesFromProduct(product)

    const image = images[0]

    const imageSource = getImageSource(image)

    function handleClick() {
        console.log(`clicked on: ${name} (id ${configId})`)
        navigate(`/configurator/${configId}`)
    }

    return (
        <ImageListItem key={imageSource} sx={{width: '100%', ':hover': {cursor: 'pointer'}}} onClick={handleClick}>
            <img
                width="100%"
                src={`${imageSource}?w=248&fit=crop&auto=format`}
                srcSet={`${imageSource}?w=248&fit=crop&auto=format&dpr=2 2x`}
                alt={name}
                loading="lazy"
            />
            <ImageListItemBar
                title={name}
                subtitle={description}
            />
        </ImageListItem>
    )
}