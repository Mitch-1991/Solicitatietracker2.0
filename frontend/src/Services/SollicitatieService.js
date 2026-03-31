const API_URL = "http://localhost:5158/api/application"

export async function createApplication(applicationData) {
    const response = await fetch (API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(applicationData)
    })
    if(!response.ok) {
        let errorMessage = "Fout bij het aanmaken van de sollicitatie."
        try{
            const errorData = await response.json()
            if(errorData?.message) {
                errorMessage = errorData.message
            }
        } catch {
            // Ignore JSON parse errors
        }
        throw new Error(errorMessage)
    }
    return await response.json()
}
export async function updateApplication(id, applicationData) {
    const response = await fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(applicationData)
    });

    if (!response.ok) {
        let errorMessage = "Fout bij het bewerken van de sollicitatie.";
        try {
            const errorData = await response.json();
            if (errorData?.message) {
                errorMessage = errorData.message;
            }
        } catch {
            // Ignore JSON parse errors
        }
        throw new Error(errorMessage);
    }

    return await response.json();
}