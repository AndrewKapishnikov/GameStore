let loginButton = document.getElementById("loginSubmit");
let orderUrl = document.getElementById("orderUrl");
if (loginButton) {
    loginButton.addEventListener("click", e => {
        e.preventDefault();
        let errors = document.getElementById("errors")
        while (errors.firstChild) errors.removeChild(errors.firstChild);
        let remember, orderValue;
        const checkbox = document.getElementsByName("RememberMe");
        if (checkbox[0].checked)
            remember = "true";
        else remember = "false";

        const form = document.forms["loginForm"];
        const returnUrl = form.elements["returnUrl"].value;
        const email = form.elements["Email"].value;
        const password = form.elements["Password"].value;
        //const remember = form.elements["RememberMe"].value;
        if (orderUrl)
            orderValue = orderUrl.value;
        SubmitLogin(email, password, remember, returnUrl, orderValue);
    });
}

async function SubmitLogin(email, password, remember, returnUrl, orderUrl) {
    
    const response = await fetch("/account/login", {
        method: "POST",
        headers: {
            "Content-type": "application/x-www-form-urlencoded; charset=UTF-8",
            "Accept": "application/json"
        },  
        body: `Email=${email}&Password=${password}&RememberMe=${remember}&returnUrl=${returnUrl}&orderUrl=${orderUrl}`,  
    });
    if (response.ok === true) {
        const res = await response.json();
        if (res.success === true)
        {
            //User confirmed and make order
            if (res.makeOrder === true) {
                let form1 = document.createElement('form');
                form1.action = '/order/makeorder';
                form1.method = 'POST';
                //form.id = "f1";

                const hiddenField1 = document.createElement('input');
                hiddenField1.type = 'hidden';
                hiddenField1.name = 'orderId';
                hiddenField1.value = res.orderId;
                form1.appendChild(hiddenField1);

                document.body.append(form1);
                form1.submit();
            }
            else {
                 //User confirmed and don't make order
                window.location.href = res.returnUrl;
            }
        }
        else
        {
            //The user is not confirmed
            let form = document.createElement('form');
            form.action = '/account/sendmessage';
            form.method = 'POST';
   

            const hiddenFieldFirst = document.createElement('input');
            hiddenFieldFirst.type = 'hidden';
            hiddenFieldFirst.name = 'orderUrl';
            hiddenFieldFirst.value = res.orderUrl;
            form.appendChild(hiddenFieldFirst);

            const hiddenFieldSecond = document.createElement('input');
            hiddenFieldSecond.type = 'hidden';
            hiddenFieldSecond.name = 'id';
            hiddenFieldSecond.value = res.id;
            form.appendChild(hiddenFieldSecond);

            document.body.append(form);
            form.submit();
        }
    }
    else {
        const errorData = await response.json();
        console.log("errors", errorData);
        if (errorData) {
            if (errorData["Email"]) {
                addError(errorData["Email"]);
            }
            if (errorData["Password"]) {
                addError(errorData["Password"]);
            }
            if (errorData["RegisterError"]) {
                addError(errorData["RegisterError"]);
            }
        }
    }
}

function addError(errors) {
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        document.getElementById("errors").append(p);
    });
};

let searchForm = document.getElementById('searchForm');
if (searchForm) {
    searchForm.addEventListener("submit",
        e => {
            let input = document.getElementById('searchinput');
            if (!input.value.trim())
                e.preventDefault();
        }
    );
}

   

