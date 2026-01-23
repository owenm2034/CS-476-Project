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
    1. Robust: The system should handle
    empty searches, and display an adequate, informative message when an invalid search 
    has been made. If for some reason an invalid combination of filters is applied – such 
    as filtering to listings less than zero dollars – the system should handle the invalid 
    filter state and provide the user with a message indicating no listings found.
    
    2. Time-efficient: When searching and filtering through listings, the page 
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
    1. Robust: If seller misses mandatory fileds (i.e. category, conditions, price), they should get an alert.
    2. Time Efficiency: Seller must be able to post items within 5 minutes (in average). Conditions are name of the item, a picture, short description, conditiona and price.
    3. Correctness: If seller decides to make modifications to the listing after initial post, the data must accurately reflect immediately.



