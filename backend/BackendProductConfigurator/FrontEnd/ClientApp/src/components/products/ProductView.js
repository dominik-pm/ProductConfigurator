import React, { useEffect } from 'react'
import Product from './Product'
import { connect } from 'react-redux'
import { Grid, Typography } from '@mui/material'
import { fetchProducts } from '../../state/product/productSlice'
import { selectProductError, selectProducts, selectProductStatus } from '../../state/product/productSelector'
import { selectLanguage } from '../../state/language/languageSelectors'
import { translate } from '../../lang'
import Loader from '../Loader'

function ProductView({ products = [], status, error, fetchProducts, language }) {

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
            <Loader></Loader>
        )
    }

    function renderEmptyProducts() {
        return (
            <Typography variant="h2">{translate('noProductsFound', language)}</Typography>
        )
    }
    function renderProducts() {
        return (
            <Grid container columns={12} spacing={2} sx={{paddingTop: 2, marginRight: 4}}>
                {products.map((product, index) => (
                    <Grid item sm={12} md={6} lg={4} xl={3} width="100%" key={index}>
                        <Product product={product}></Product>
                    </Grid>
                ))}
            </Grid>
        )
    }

    function renderApiFailed(errorMessage) {
        return (
            <div>
                <Typography variant="h2">{translate('failedToLoadProducts', language)}</Typography>
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