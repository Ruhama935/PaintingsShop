﻿const signUp = async () => {
    const userName = document.getElementById("userName").value;
    const password = document.getElementById("password").value;
    const firstName = document.getElementById("firstName").value;
    const lastName = document.getElementById("lastName").value;
    const response = await fetch("https://localhost:44304/api/users", {
        method: 'POST',
        body: JSON.stringify({ userName, password, firstName, lastName }),
        headers: {
            "Content-Type": 'application/json'
        }
    })
    if (!response.ok)
        throw new Error("Error in the server")
    const user = await response.json();
    alert(`welcome ${firstName}! your registration was successful. Please log in to enter.`)
    window.location.assign('./login.html')
}

const login = async () => {
    const userName = document.getElementById("userName").value;
    const password = document.getElementById("password").value;
    const response = await fetch("https://localhost:44304/api/users/login", {
        method: 'POST',
        body: JSON.stringify({ userName, password }),
        headers: {
            "Content-Type": 'application/json'
        }
    })
    if (!response.ok)
        throw new Error("Error in the server")
    const user = await response.json();
    window.location.assign('./in.html')
}