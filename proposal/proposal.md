# Project Title: Campus Swap(?) Dorm Shop(?)

- Owen Monus
- Cole Yager
- Pawan Kshetri
- Weicheng Wang
- Pushpinder Singh
- Esteban Perez Mendoza

## Well Written Problem Description.
In university housing, students living away from home struggle to buy and sell dorm
furnishings due to quick semester changeovers and a lack of transportation access. This
causes students to buy and sell items at pawn shops and online marketplaces where sellers
are trying to make money, not help students. This adds stress to students, who are already
financially burdened and are typically on a short timeline to move in or out before the
next semester begins. The goal of this project is to create an online marketplace where
students can buy and sell housing furnishings to other students at the same campus,
eliminating transportation issues and allowing students to support other students.

### Two User Roles
1. Buyer
    #### Main Functional Requirements
    1. Buyer can view and search for items.
    2. Buyer can use filters to view and sort listings by category, condition, and price.
    3. Buyer can communicate with a seller for more details about a certain listing.
    4. Buyer can communicate with more than one seller regarding more than one listing at a time.
    5. Buyer can receive email alerts if seller responds to their messages.
    6. Buyer can save listings to view at a later time.
    7. Buyer has a profile where they can save listings and access their chats.

    #### Main Quality Requirements
    1. Robustness: The system should handle
    empty searches, and display an adequate, informative message when an invalid search
    has been made. If for some reason an invalid combination of filters is applied – such
    as filtering to listings less than zero dollars – the system should handle the invalid
    filter state and provide the user with a message indicating no listings found.

    2. Time Efficiency: When searching and filtering through listings, the page
    must load within 2 seconds. When communicating with sellers, messages should send
    immediately and messages from the seller should be fetched every 15 seconds. The buyer
    should receive emails notifying them of seller communication within 2 minutes of the
    server receiving a message from the seller. When viewing a listing, the page should
    load in less than 4 seconds including images. When a buyer saves a listing for later,
    it should be almost instant. The buyer should also be able to load their profile and
    access their saved items and chats in less than 5 seconds.

    3. Correctness: If a seller marks an item as sold while buyer is viewing an item, this
    should reflect immediately on the buyer's end without refreshing the page. When a
    buyer searches for a listing, the search should be deterministic and accurate. The
    filters should work properly in conjunction with a search. When a buyer is
    communicating with a seller, their messages go to the intended recipient only. When a
    buyer receives a message, it is displayed from the correct sender. Upon saving a
    listing, that listing is saved on the buyers account only. A buyer should not receive
    an email intended for another user.

2. Seller
    #### Main Functional Requirements
    1. Seller can create a listing. (must specify category, conditions, price)
    2. Seller can edit a listing (includes on hold, sold, other modifications to existing listing).
    3. Seller can delete a listing.
    4. Seller can communicate with a buyer to give more details about a listing.
    5. Seller can communicate with more than one buyer regarding more than one listing at a time.
    6. Seller can receive email alerts for communication with buyers.
    7. Seller has a profile where they can see their listings and access their chats.

    #### Main Quality Requirements
    1. Robustness: The system should provide field validation when making a listing. If the
    seller misses filling mandatory fields (such as category, conditions, price), they
    should not be able to create the listing. The system should prevent a listing from
    being edited into an invalid state, such as having a negative price, or no category.
    The system should handle photo uploads of various file types and ensure the image is
    not too large to save easily. The system should present the seller with informative
    error messages indicating the error state.

    2. Time Efficiency: Seller should be able to post a listing within 5 seconds. Attached
    pictures should upload quickly – within 2 seconds per picture. The UI for making a
    listing should feel responsive, and all loading screens should be less than 5 seconds.
    Editing a listing should take less than 2 seconds. The seller should be able to load
    their profile within two seconds. When communicating with buyers, messages should send
    immediately and messages from the buyer should be fetched every 15 seconds. The seller
    should receive emails notifying them of buyer communication within 2 minutes of the
    server receiving a message from the buyer.

    3. Correctness: A seller's listing should be presented to buyers in the exact way they
    created it. Any modifications to a listing made by a seller should update immediately 
    so buyers see the most up to date information. When a seller uploads images, the 
    images should be stored without loss of data. The images should be able to be 
    retrieved at a later time. When a seller deletes a listing, only the selected listing 
    is deleted. When a seller is communicating with a buyer, their messages go to the 
    intended recipient only. When a seller receives a message, it is displayed from the 
    correct sender.



