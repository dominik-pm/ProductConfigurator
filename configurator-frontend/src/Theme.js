import { createTheme, responsiveFontSizes } from '@mui/material'

const theme = responsiveFontSizes(createTheme())

theme.palette = {
    ...theme.palette
}

theme.typography = {
    ...theme.typography
}


// const coef = 0.1
// const modifyRem = (value, coef) => {
//     return `${parseFloat(value) * (1 + coef)}rem`
// }

// each(theme.typography, (variant, variantName) => {
//     if (typeof variant !== 'object') {
//         return variant
//     }
//     theme.typography[variantName] = {
//         ...variant,
//         fontSize: modifyRem(variant.fontSize, -coef * 5),
//         [theme.breakpoints.up('sm')]: {
//             fontSize: modifyRem(variant.fontSize, -coef * 2.5),
//         },
//         [theme.breakpoints.up('md')]: {
//             fontSize: modifyRem(variant.fontSize, -coef * 1),
//         },
//         [theme.breakpoints.up('lg')]: {
//             fontSize: modifyRem(variant.fontSize, 0),
//         },
//         [theme.breakpoints.up('xl')]: {
//             fontSize: modifyRem(variant.fontSize, coef),
//         },
//     }
// })

export default theme