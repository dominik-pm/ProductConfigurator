---
name: Task (Generic)
about: Description of a generic task
title: ''
labels: ''
assignees: ''

---

# Frontend Product listings
> list all products on the front page of the website

**Expected Behaviour**
> on the front page of the webinterface, there should be a listing of all available products
> 
> only the name, description (TODO: and models/preconfigs) and an image is shown
> 
> if the user clicks on a product, the detailed page of the product is shown (with all the configuration options)

**Todo:**
- [ ] implement an http-request that gets all the products

**Test cases:**
- [ ] test error handling when requesting the data
    - [ ] when no connection to the backend can be established (timeout)
    - [ ] when the wrong data is sent or is in the wrong format
- [ ] multiple products can be displayed
