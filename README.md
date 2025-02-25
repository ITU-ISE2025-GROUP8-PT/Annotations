# Annotations
Project to build a medical image annotation app especially for Angiography. 

# Contribution guidelines
## Branching Strategy
![Example of gitflow branching strategy](https://media.discordapp.net/attachments/1313137352125714433/1343904612364714087/image.png?ex=67bef834&is=67bda6b4&hm=7270b2583df9af0e22d6b298d419960693b5ec8044661f34e5d47d6c0bbaa6f9&=&format=webp&quality=lossless&width=904&height=662)

Development of new code is produced in feature branches, which is added to the development branch when meeting acceptance criteria.
The development branch gets merged with the main branch upon review and approval by the stakeholders and the company. <p></p> The main and development branch is kept safe by only allowing merges through reviewed pull requests.

## Branch Naming Conventions
All branches follow the convention of

```bash
feature/[function_of_the_feature]/[possible_version_number]

bugfix/[what_to_fix]

refactor/[what_to_refactor]  

test/[what_to_test]
```

Example of naming branches: "feature/Upload_IMG" , "refactor/file_System" , "test/user_registration"

## Commit Message Convention
The title is no longer than 50 characters. A description is only neeed when the commit is complicated. There is an empty line between title and description(if needed) and cou-authors.
 <p></p> 
The co-autor follows the following patterns

```bash
"Co-authored-by: firstname lastname <my@githubmail.com>"
```
Example of commit message:

```bash
"Added image upload functionality.

A user can now upload their Images through the specified user interface. <p> </p>

Co-authored-by: Hans Christiansen <hc@gmail.com>"
```

