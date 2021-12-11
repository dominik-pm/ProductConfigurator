import React from 'react'
import Product from './Product'
import { useSelector } from 'react-redux'
import { Typography } from '@mui/material'

export default function ProductView() {

    const { products, status, error } = useSelector(state => state.product)
    const isEmpty = products.length === 0 ? true : false

    function render() {
        switch (status) {
            case 'loading':
                return renderLoadingProducts()
            case 'succeeded':
                return (isEmpty ? renderEmptyProducts() : renderProducts())
            case 'failed':
                return renderApiFailed(error)
            default:
                return renderLoadingProducts()
        }
    }

    function renderLoadingProducts() {
        return (
            <Typography variant="h2">Loading Products...</Typography>
        )
    }

    function renderEmptyProducts() {
        return (
            <Typography variant="h2">No products found</Typography>
        )
    }
    function renderProducts() {
        return (
            <div className="ProductContainer">
                {
                    products.map(product => (
                        <Product key={product.id} product={product}></Product>
                    ))
                }
            </div>
        )
    }

    function renderApiFailed(errorMessage) {
        return (
            <div>
                <Typography variant="h1">Failed to load products!</Typography>
                <Typography variant="p">{errorMessage}</Typography>
            </div>
        )
    }
    
    return (
        render()
    )
}
