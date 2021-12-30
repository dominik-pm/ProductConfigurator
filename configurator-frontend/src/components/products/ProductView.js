import React, { useEffect } from 'react'
import Product from './Product'
import { connect } from 'react-redux'
import { Typography } from '@mui/material'
import { fetchProducts } from '../../state/product/productSlice'
import { selectProductError, selectProducts, selectProductStatus } from '../../state/product/productSelector'
import { selectLanguage } from '../../state/language/languageSelectors'
import { translate } from '../../lang'
import './ProductView.css'

function ProductView({ products, status, error, fetchProducts, language }) {

    const isEmpty = products.length === 0 ? true : false

    useEffect(() => {
        if (isEmpty) {
            console.log('calling to fetch products...')
            fetchProducts()
        }
    }, [fetchProducts, isEmpty])

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
            <Typography variant="h2">{translate('loadingProducts', language)}...</Typography>
        )
    }

    function renderEmptyProducts() {
        return (
            <Typography variant="h2">{translate('noProductsFound', language)}</Typography>
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
                <Typography variant="h1">{translate('failedToLoadProducts', language)}</Typography>
                <Typography variant="body1">{translate(errorMessage, language)}</Typography>
            </div>
        )
    }
    
    return (
        render()
    )
}

const mapStateToProps = (state) => ({
    products: selectProducts(state),
    status: selectProductStatus(state),
    error: selectProductError(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    fetchProducts
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ProductView)