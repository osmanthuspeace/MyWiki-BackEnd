const url="https://localhost:7100/api/User"
let characters=[]

let u=document.querySelector("#number");
let p=document.querySelector("#pass");

const register=() => {
    let obj={
        "name":u.value,
        "password":p.value
    }
    fetch(url,{
        method:"post",
        headers:{
            "Content-Type": "application/json"
        },
        body:JSON.stringify(obj)
    }).then(res=>res.json())
        .then(data=>console.log(data))
        .catch(e=>console.error("wrong",e));
    window.open("https://localhost:7100/api/Character")
}

const login = (name,pass) => {
    let obj={
        "name":name,
        "password":pass
    }
    fetch(url,{
        method:"post",
        headers:{
            "Content-Type":"application/json",
        },
        body:JSON.stringify(obj),
    }).then(res=>res.json())
        .then(data=>{
            console.log("Login successful:",data);
            window.open("https://localhost:7100/api/User");
        })
        .catch(e=>{
            console.error("Login failed:",e);
        })
}





function jump(){
    var inputt = document.getElementById("number");
    var i = inputt.value;
    if (!/B2304+.*/.test(i)) {
        alert("请输入正确学号");
    }
    else{
        window.location.href="./page/WikiMainPage.html";
    }
}