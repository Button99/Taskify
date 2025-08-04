
const apiBase = "http://localhost:5054/api/tasks"; // Adjust to your actual backend

const TaskStatus = ["Pending", "In Progress", "Completed", "Cancelled"];
const TaskPriority = ["Low", "Normal", "High", "Urgent"];
const tokenKey = "taskify_token";

// Auth helpers
function isLoggedIn() {
  return !!localStorage.getItem(tokenKey);
}

function authHeader() {
  const token = localStorage.getItem(tokenKey);
  return { Authorization: `Bearer ${token}` };
}

// Load all tasks
async function loadTasks() {
  const res = await fetch(apiBase, {
    headers: authHeader()
  });

  if (!res.ok) {
    if (res.status === 401) {
      alert("Session expired. Please log in again.");
      localStorage.removeItem(tokenKey);
      location.reload();
    }
    return;
  }

  const tasks = await res.json();
  const list = document.getElementById("task-list");
  list.innerHTML = "";

  tasks.forEach(task => {
    const li = document.createElement("li");
    li.className = "bg-gray-50 p-3 rounded shadow-sm border";

    li.innerHTML = `
      <div class="flex justify-between">
        <div>
          <strong>${task.title}</strong><br/>
          <small>${task.description || ""}</small><br/>
          <small>Status: ${TaskStatus[task.status]}</small> | 
          <small>Priority: ${TaskPriority[task.priority]}</small><br/>
          ${task.dueDate ? `<small>Due: ${task.dueDate.split("T")[0]}</small><br/>` : ""}
        </div>
        <div class="flex flex-col gap-1 items-end">
          <button class="text-sm text-green-600" onclick="toggleStatus(${task.id}, ${task.status})">
            ${task.status === 2 ? "Undo" : "Mark Done"}
          </button>
          <button class="text-sm text-red-500" onclick="deleteTask(${task.id})">🗑️</button>
        </div>
      </div>
    `;

    list.appendChild(li);
  });
}

// Toggle task completion status
async function toggleStatus(id, currentStatus) {
  const newStatus = currentStatus === 2 ? 0 : 2;
  await fetch(`${apiBase}/${id}`, {
    method: "PATCH",
    headers: {
      ...authHeader(),
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ status: newStatus })
  });
  loadTasks();
}

// Delete a task
async function deleteTask(id) {
  await fetch(`${apiBase}/${id}`, {
    method: "DELETE",
    headers: authHeader()
  });
  loadTasks();
}

// Handle task form submission
document.getElementById("task-form").addEventListener("submit", async e => {
  e.preventDefault();

  const title = document.getElementById("task-title").value;
  const description = document.getElementById("task-desc").value;
  const dueDate = document.getElementById("task-date").value;
  const status = parseInt(document.getElementById("task-status").value);
  const priority = parseInt(document.getElementById("task-priority").value);

  await fetch(apiBase, {
    method: "POST",
    headers: {
      ...authHeader(),
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      title,
      description,
      dueDate: dueDate || null,
      status,
      priority
    })
  });

  e.target.reset();
  document.getElementById("task-priority").value = "1";
  document.getElementById("task-status").value = "0";
  loadTasks();
});

// Handle login
document.getElementById("login-form").addEventListener("submit", async e => {
  e.preventDefault();

  const username = document.getElementById("login-username").value;
  const password = document.getElementById("login-password").value;

  const res = await fetch("http://localhost:5054/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password })
  });

  if (res.ok) {
    const data = await res.json();
    localStorage.setItem(tokenKey, data.token);
    document.getElementById("auth-section").classList.add("hidden");
    document.getElementById("task-section").classList.remove("hidden");
    loadTasks();
  } else {
    alert("Login failed.");
  }
});

// Handle registration
document.getElementById("register-form").addEventListener("submit", async e => {
  e.preventDefault();

  const email = document.getElementById("reg-email").value;
  const username = document.getElementById("reg-username").value;
  const password = document.getElementById("reg-password").value;

  const res = await fetch("http://localhost:5054/api/auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, username, password })
  });

  if (res.ok) {
    alert("Registration successful. Please log in.");
    showAuth(true);
  } else {
    alert("Registration failed.");
  }
});

// Toggle between login/register
document.getElementById("toggle-auth").addEventListener("click", () => {
  const loginVisible = !document.getElementById("login-form").classList.contains("hidden");
  showAuth(!loginVisible);
});

// Show login or register form
function showAuth(showLogin = true) {
  document.getElementById("auth-section").classList.remove("hidden");
  document.getElementById("task-section").classList.add("hidden");
  document.getElementById("login-form").classList.toggle("hidden", !showLogin);
  document.getElementById("register-form").classList.toggle("hidden", showLogin);
}

// Logout
document.getElementById("logout-btn").addEventListener("click", () => {
  localStorage.removeItem(tokenKey);
  location.reload();
});

// Initial page load logic
if (isLoggedIn()) {
  document.getElementById("auth-section").classList.add("hidden");
  document.getElementById("task-section").classList.remove("hidden");
  loadTasks();
} else {
  showAuth(true);
}
