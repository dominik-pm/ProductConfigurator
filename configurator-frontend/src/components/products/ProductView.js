import React, { useEffect } from 'react'
import Product from './Product'
import { useDispatch, useSelector } from 'react-redux'
import { Typography } from '@mui/material'
import { fetchProducts } from '../../state/product/productSlice'
import './ProductView.css'

export default function ProductView() {
    const dispatch = useDispatch()

    const { products, status, error } = useSelector(state => state.product)
    const isEmpty = products.length === 0 ? true : false

    useEffect(() => {
        if (isEmpty) {
            console.log('calling to fetch products...')
            dispatch(fetchProducts())
        }
    }, [dispatch, isEmpty])

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
